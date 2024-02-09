using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Guest;
using TinyPath.Application.Services.Hangfire;
using TinyPath.Application.Services.Link;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Application.Logic.Link;

public abstract class CreateShortLinkCommand
{
    public class Request : IRequest<Response>
    {
        public required string Url { get; init; } 
    }
    
    public class Response
    {
        public string Link { get; set; } = string.Empty;
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly ILinkManager _linkManager;
        private readonly IGetLinkOptions _getLinkOptions;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IGuestManager _guestManager;
        private readonly IBackgroundServices _backgroundServices;
        
        public Handler(IApplicationDbContext dbContext, ILinkManager linkManager, IGetLinkOptions getLinkOptions, ICurrentUserProvider currentUserProvider, IGuestManager guestManager, IBackgroundServices backgroundServices) : base(dbContext)
        {
            _linkManager = linkManager;
            _getLinkOptions = getLinkOptions;
            _currentUserProvider = currentUserProvider;
            _guestManager = guestManager;
            _backgroundServices = backgroundServices;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var maxLinkCountForGuest = _getLinkOptions.GetMaxLinkCountForGuestUser();
            var blockTimeInMinutesForGuest = _getLinkOptions.GetBlockTimeInMinutesForGuestUser();

            var user = await _currentUserProvider.GetAuthenticatedUser();
            
            var (fullLink, linkCode) = _linkManager.GenerateShortLink(request.Url);
            
            var isNotUnique = await _dbContext.Links
                .AnyAsync(l => l.Code == linkCode);

            if (isNotUnique)
            {
                throw new ErrorException("LinkAlreadyExists");
            }
            
            var linkEntity = new Domain.Entities.TinyPath.Link
            {
                Code = linkCode,
                Url = fullLink,
                OriginalUrl = request.Url,
            };

            if (user is null)
            {
                var guest = await _guestManager.GetGuestUser();

                if (guest is null)
                {
                    var createdGuest = await _guestManager.CreateGuestUser();
                    guest = createdGuest;
                }
                else
                {
                    _guestManager.ValidateGuestUserCreationLink(guest);
                }
                
                linkEntity.GuestId = guest.Id;
                linkEntity.Guest = guest;
                guest.Links++;
                
                if (guest.Links >= maxLinkCountForGuest)
                {
                    guest.Blocked = true;
                    guest.BlockedUntil = DateTimeOffset.UtcNow.AddMinutes(blockTimeInMinutesForGuest);
                    _backgroundServices.UnblockGuestUser(guest.Id, blockTimeInMinutesForGuest);
                }
                
                _dbContext.Guests.Update(guest);
            }
            else
            {
                linkEntity.UserId = user.Id;
                linkEntity.User = user;
            }
            
            _dbContext.Links.Add(linkEntity);
            
            var linkStatsEntity = new LinkStats
            {
                LinkId = linkEntity.Id,
                Link = linkEntity
            };
            
            _dbContext.LinksStats.Add(linkStatsEntity);

            await _dbContext.SaveChangesAsync();
            
            return new Response
            {
                Link = fullLink
            };
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
        }
    }
}