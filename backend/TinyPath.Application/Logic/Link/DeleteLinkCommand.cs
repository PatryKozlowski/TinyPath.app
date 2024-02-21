using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.Link;

public abstract class DeleteLinkCommand
{
    public class Request : IRequest<Response>
    {
        public string LinkId { get; set; }
    }
    
    public class Response
    {
        public bool Success { get; set; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        public Handler(IApplicationDbContext dbContext, ICurrentUserProvider currentUserProvider) : base(dbContext)
        {
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _currentUserProvider.GetAuthenticatedUser();
            
            if (user is null)
            {
                throw new UnauthorizedException();
            }
            
            if (Guid.TryParse(request.LinkId, out Guid guidId))
            {
                var data = await _dbContext.Links
                    .Where(x => x.UserId == user.Id && x.Id == guidId)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (data is not null)
                {
                    _dbContext.Links.Remove(data);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return new Response() { Success = true };
                }
            }

            throw new ErrorException("LinkNotFound");
        }
    }
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.LinkId)
                .NotEmpty().WithMessage("LinkIdIsRequired");
        }
    }
}