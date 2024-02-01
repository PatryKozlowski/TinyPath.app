using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

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
        
        public Handler(IApplicationDbContext dbContext, IJwtManager jwtManager, IAuthDataProvider authDataProvider, IGetJwtOptions getJwtOptions) : base(dbContext)
        {
            _jwtManager = jwtManager;
            _authDataProvider = authDataProvider;
            _getJwtOptions = getJwtOptions;
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
                    .Where(x => x.RefreshToken.Expires > DateTimeOffset.UtcNow)
                    .Include(us => us.Session)
                    .FirstOrDefaultAsync(u => u.Id == userId.Value);

                if (user is not null)
                {
                    var userRefreshToken = user.RefreshToken;
                    
                    if (userRefreshToken.Token == refreshToken)
                    {
                        var token = _jwtManager.GenerateToken(user.Id, user.Session.Id);
                        var expiration = _getJwtOptions.GetExpirationTokenTime(true);
                        
                        var newRefreshTokenEntity = new Domain.Entities.RefreshToken()
                        {
                            Token = token,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(expiration),
                            User = user
                        };
                        
                        _dbContext.RefreshTokens.Update(newRefreshTokenEntity);
                        
                        await _dbContext.SaveChangesAsync();
                        
                        return new Response() { Token = token };
                    }
                }
            }
            
            throw new UnauthorizedException("InvalidRefreshToken");
        }
    }
}