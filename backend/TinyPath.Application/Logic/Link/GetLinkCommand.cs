using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.Link;

public class GetLinkCommand
{
    public class Request : IRequest<Response>
    {
        public string LinkId { get; set; }
    }
    
    public class Response
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string OrginalUrl { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; }
        public bool Active { get; set; }
        public bool IsCustom { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        public readonly ICurrentUserProvider _currentUserProvider;
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
                    .FirstOrDefaultAsync();
                
                return new Response() { 
                    Id = data!.Id,
                    Url = data.Url,
                    OrginalUrl = data.OriginalUrl,
                    Title = data.Title,
                    Code = data.Code,
                    Active = data.Active,
                    IsCustom = data.Custom,
                    Created = data.Created,
                    Updated = data.Updated};
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