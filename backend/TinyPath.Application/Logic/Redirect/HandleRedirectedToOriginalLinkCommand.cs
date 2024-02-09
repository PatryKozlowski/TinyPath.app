using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Redirect;

namespace TinyPath.Application.Logic.Redirect;

public abstract class HandleRedirectedToOriginalLinkCommand
{
    public class Request : IRequest<Response>
    {
        public required string LinkCode { get; init; } 
    }
    
    public class Response
    {
        public string Link { get; init; } = string.Empty;
        public required int StatusCode { get; init; }

    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IRedirectManager _redirectManager;
        
        public Handler(IApplicationDbContext dbContext, IRedirectManager redirectManager) : base(dbContext)
        {
            _redirectManager = redirectManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var link = await _dbContext.Links
                .Where(x => x.Code == request.LinkCode)
                .Where(x => x.Active)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (link == null)
            {
                throw new ErrorException("LinkNotFound");
            }
            
            var originalLink = await _redirectManager.HandleRedirectLink(request.LinkCode);

            await _redirectManager.UpdateLinkStatsAfterRedirect(link.Id);
            
            return new Response
            {
                Link = originalLink,
                StatusCode = 302
            };
        }
    }
}