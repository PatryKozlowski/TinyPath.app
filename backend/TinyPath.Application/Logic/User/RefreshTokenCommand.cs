using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Hangfire;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Application.Logic.User;

public abstract class RefreshTokenCommand
{
    public class Request : IRequest<Response>
    {
    }
    
    public class Response
    {
        public required string Token { get; init; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IJwtManager _jwtManager;
        private readonly IAuthDataProvider _authDataProvider;
        private readonly IGetJwtOptions _getJwtOptions;
        private readonly IBackgroundServices _backgroundServices;
        
        public Handler(IApplicationDbContext dbContext, IJwtManager jwtManager, IAuthDataProvider authDataProvider, IGetJwtOptions getJwtOptions, IBackgroundServices backgroundServices) : base(dbContext)
        {
            _jwtManager = jwtManager;
            _authDataProvider = authDataProvider;
            _getJwtOptions = getJwtOptions;
            _backgroundServices = backgroundServices;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _authDataProvider.GetUserId(true);
            var refreshToken = _authDataProvider.GetRefreshTokenFromCookie();
            
            if (userId.HasValue && refreshToken is not null)
            { 
                _jwtManager.ValidateToken(refreshToken);
                
                var user = await _dbContext.Users
                    .Include(urf => urf.RefreshToken)
                    .Include(s => s.Session)
                    .Where(x => x.RefreshToken.Expires > DateTimeOffset.UtcNow)
                    .FirstOrDefaultAsync(u => u.Id == userId.Value);

                if (user is not null)
                {
                    if (user.Session is not null)
                    {
                        throw new UnauthorizedException("UserAlreadyHasActiveSession");
                    }
                    
                    var userRefreshToken = user.RefreshToken;
                    
                    if (userRefreshToken.Token == refreshToken)
                    {
                        var expirationSession = _getJwtOptions.GetExpirationTokenTime(false);
                        
                        var newUserSession = new Session()
                        {
                            User = user,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(expirationSession)
                        };
                        
                        _dbContext.Sessions.Add(newUserSession);
                        
                        var token = _jwtManager.GenerateToken(user.Id, newUserSession.Id);
                        
                        var jobId = _backgroundServices.DeleteExpiredSessions(newUserSession.Id, expirationSession);
                        
                        newUserSession.HangfireId = jobId;
                        
                        await _dbContext.SaveChangesAsync();
                        
                        return new Response() { Token = token };
                    }
                }
            }
            
            throw new UnauthorizedException("InvalidRefreshToken");
        }
    }
}