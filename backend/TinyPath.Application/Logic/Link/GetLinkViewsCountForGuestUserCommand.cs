using MediatR;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Link;

namespace TinyPath.Application.Logic.Link;

public abstract class GetLinkViewsCountForGuestUserCommand
{
    public class Request : IRequest<Response>
    {
        public required string LinkId { get; init; } 
    }
    
    public class Response
    {
        public int LinkViewsCount { get; set; }
        public string LinkUrl { get; set; } = string.Empty;
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly ILinkManager _linkManager;
        
        public Handler(IApplicationDbContext dbContext, ILinkManager linkManager) : base(dbContext)
        {
            _linkManager = linkManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var parsedLinkId = Guid.TryParse(request.LinkId, out var linkId);
            
            if (!parsedLinkId)
            {
                throw new ErrorException("InvalidLinkId");
            }
            
            var linkViewsCount = await _linkManager.GetLasGuestLinkVisitsByLinkId(linkId);
            
            return new Response() { LinkViewsCount = linkViewsCount.totalVisits, LinkUrl = linkViewsCount.linkUrl! };
        }
    }
}