using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;

namespace TinyPath.Application.Logic.Link;

public abstract class GetLinkStatsCommand
{
    public class Request : IRequest<Response>
    {
        public string LinkId { get; set; }
    }

    public class Response
    {
        public int Visits { get; set; }
        public List<string> Browser { get; set; } = new List<string>();
        public List<string> Device { get; set; } = new List<string>();
        public List<string> Platform { get; set; } = new List<string>();
        public List<string> Country { get; set; } = new List<string>();
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        public Handler(IApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.LinkId, out Guid guidLinkId))
            {
                throw new ErrorException("InvalidLinkId");
            }

            var dataFromLinkStats = await _dbContext.LinksStats
                .Where(x => x.LinkId == guidLinkId)
                .FirstOrDefaultAsync();

            if (dataFromLinkStats is null)
            {
                throw new ErrorException("LinkNotFound");
            }
            
            var response = new Response
            {
                Visits = dataFromLinkStats.Visits,
                Browser = new List<string>(dataFromLinkStats.Browser),
                Device = new List<string>(dataFromLinkStats.Device),
                Platform = new List<string>(dataFromLinkStats.Platform),
                Country = new List<string>(dataFromLinkStats.Country)
            };
            
            var temporaryStats = await _dbContext.TemporaryLinkStats
                .Where(tempStats => tempStats.LinkId == guidLinkId)
                .GroupBy(tempStats => tempStats.LinkId)
                .Select(group => new
                {
                    Visits = group.Sum(tempStats => tempStats.Visits),
                    Browser = group.Select(tempStats => tempStats.Browser).ToList(),
                    Device = group.Select(tempStats => tempStats.Device).ToList(),
                    Platform = group.Select(tempStats => tempStats.Platform).ToList(),
                    Country = group.Select(tempStats => tempStats.Country).ToList()
                })
                .FirstOrDefaultAsync();


            if (temporaryStats != null)
            {
                response.Visits += temporaryStats.Visits;
                response.Browser.AddRange(temporaryStats.Browser);
                response.Device.AddRange(temporaryStats.Device);
                response.Platform.AddRange(temporaryStats.Platform);
                response.Country.AddRange(temporaryStats.Country);
            }

            return response;
            
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
}