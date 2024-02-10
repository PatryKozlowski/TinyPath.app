using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Services.Link;

namespace TinyPath.Application.Services.Guest;

public class GuestManager : IGuestManager
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IAuthDataProvider _authDataProvider;
    private readonly IGetLinkOptions _getLinkOptions;

    public GuestManager(IApplicationDbContext dbContext, IAuthDataProvider authDataProvider, IGetLinkOptions getLinkOptions)
    {
        _dbContext = dbContext;
        _authDataProvider = authDataProvider;
        _getLinkOptions = getLinkOptions;
    }

    public async Task<Domain.Entities.TinyPath.Guest> CreateGuestUser()
    {
        var remoteIpAddress = GetRemoteIpAddress();
        
        var guestUserEntity = new Domain.Entities.TinyPath.Guest
        {
            IpAddress = remoteIpAddress
        };
        
        _dbContext.Guests.Add(guestUserEntity);
        
        await _dbContext.SaveChangesAsync();
        
        return guestUserEntity;
    }

    public async Task<Domain.Entities.TinyPath.Guest?> GetGuestUser()
    {
        var remoteIpAddress = GetRemoteIpAddress();
        var ipAddress = remoteIpAddress;
        
        if (remoteIpAddress is "::1")
        {
            ipAddress =  "127.0.0.1";
        }
        
        var guestUser = await _dbContext.Guests
            .Where(x => x.IpAddress == ipAddress)
            .FirstOrDefaultAsync();
        
        return guestUser;
    }
    
    public bool ValidateGuestUserCreationLink(Domain.Entities.TinyPath.Guest? guestUser)
    {
        var maxLinkCountForGuestUser = _getLinkOptions.GetMaxLinkCountForGuestUser();
        var blockTimeInMinutesForGuestUser = _getLinkOptions.GetBlockTimeInMinutesForGuestUser();
        
        if (guestUser is not null && guestUser.Links >= maxLinkCountForGuestUser || guestUser!.BlockedUntil > DateTimeOffset.UtcNow || guestUser.Blocked)
        {
            throw new ErrorException("GuestUserLinkCountExceededBlockTime " + guestUser.BlockedUntil?.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        return true;
    }

    public async Task UnblockGuestUser(Guid guestId)
    {
        var guestUser = await _dbContext.Guests
            .Where(x => x.Id == guestId)
            .FirstOrDefaultAsync();
        
        if (guestUser is not null)
        {
            guestUser.Blocked = false;
            guestUser.BlockedUntil = null;
            guestUser.Links = 0;
            
            await _dbContext.SaveChangesAsync();
        }
    }

    private string GetRemoteIpAddress()
    {
        var remoteIpAddress = _authDataProvider.GetRemoteIpAddress();

        return remoteIpAddress;
    }
}