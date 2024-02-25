using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Link;
using TinyPath.Domain.Entities.TinyPath;
using TinyPath.Domain.Enums;

namespace TinyPath.Application.Logic.Link;

public abstract class CreateCustomShortLinkCommand
{
    public class Request : IRequest<Response>
    {
        public required string Url { get; init; } 
        public required string CustomLink { get; init; }
        public string? Title { get; init; }
    }
    
    public class Response
    {
        public string Link { get; set; } = string.Empty;
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ILinkManager _linkManager;
        private readonly IGetLinkOptions _getLinkOptions;
        
        public Handler(IApplicationDbContext dbContext, ICurrentUserProvider currentUserProvider, ILinkManager linkManager, IGetLinkOptions getLinkOptions) : base(dbContext)
        {
            _currentUserProvider = currentUserProvider;
            _linkManager = linkManager;
            _getLinkOptions = getLinkOptions;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var isCustomLinkEnabled = _getLinkOptions.GetIsCustomShortLinkEnabled();
            
            if (!isCustomLinkEnabled)
            {
                throw new ErrorException("CustomLinkDisabled");
            }
            
            var isLogged = await _currentUserProvider.GetAuthenticatedUser();

            if (isLogged is null)
            {
                throw new UnauthorizedException();
            }
            
            var user = await _currentUserProvider.GetPremiumUser();

            if (user is null && user?.Role != UserRole.Admin)
            {
                throw new UnauthorizedException("OnlyPremiumUsersCanCreateCustomLinks");
            }
            
            var maxCustomLinkLength = _getLinkOptions.GetCustomShortLinkLength();
            
            if (request.CustomLink.Length > maxCustomLinkLength)
            {
                throw new ErrorException($"CustomLinkMaxLengthIs{maxCustomLinkLength}");
            }

            var shortLinkCodeExists = await _dbContext.Links
                .AnyAsync(x => x.Code == request.CustomLink);

            if (shortLinkCodeExists)
            {
                throw new ErrorException("CustomLinkAlreadyExists");
            }
            
            var (fullLink, linkCode) = _linkManager.GenerateShortLink(request.Url, request.CustomLink, true);
            
            var link = new Domain.Entities.TinyPath.Link
            {
                Code = linkCode,
                Url = fullLink,
                OriginalUrl = request.Url,
                Custom = true,
                User = user,
                Title = request.Title
            };
            
            _dbContext.Links.Add(link);
            
            var linkStats = new LinkStats()
            {
                Link = link,
                LinkId = link.Id
            };
            
            _dbContext.LinksStats.Add(linkStats);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return new Response() { Link = link.Url };
        }
    }
    
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("UrlRequired")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("InvalidURL")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)).WithMessage("InvalidUrlFormat")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute) && (url.StartsWith("http://") || url.StartsWith("https://")))
                .WithMessage("UrlMustStartWithHttpOrHttps");

            RuleFor(x => x.CustomLink)
                .NotEmpty().WithMessage("CustomCodeRequired");
        }
    }
}