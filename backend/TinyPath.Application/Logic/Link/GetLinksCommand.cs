using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Application.Logic.Link;

public abstract class GetLinksCommand
{
    public class Request : IRequest<Response>
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
    
    public class Response
    {
        public List<GetLinks> Urls { get; set; } = new List<GetLinks>();
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
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
            
            var totalRecords = await _dbContext.Links
                .Where(x => x.UserId == user.Id)
                .CountAsync(cancellationToken);
            
            var totalPages = (int)Math.Ceiling((decimal)totalRecords / request.PageSize);

            var data = await _dbContext.Links
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Created)
                .Select(x => new GetLinks
                {
                    Id = x.Id,
                    Url = x.Url,
                    Title = x.Title,
                    Active = x.Active,
                    IsCustom = x.Custom
                })
                .Skip((request.PageNo - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            return new Response() { Urls = data, TotalPages = totalPages, TotalRecords = totalRecords };
        }
    }
    
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PageNo)
                .GreaterThan(0)
                .WithMessage("PageNoMustBeGreaterThanZero");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("PageSizeMustBeGreaterThanZero");
        }
    }
}