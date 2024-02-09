using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Interfaces;
using TinyPath.Domain.Entities;
using TinyPath.Domain.Entities.TinyPath;
using TinyPath.Domain.Enums;

namespace TinyPath.Application.Services;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IAuthDataProvider _authDataProvider;

    public CurrentUserProvider(IApplicationDbContext dbContext, IAuthDataProvider authDataProvider)
    {
        _dbContext = dbContext;
        _authDataProvider = authDataProvider;
    }

    public async Task<User?> GetAuthenticatedUser()
    {
        var userSessionId = GetSessionUserId();

        var user = await _dbContext.Sessions
            .Include(s => s.User)
            .Where(s => s.Id == userSessionId)
            .Where(s => s.Expires > DateTimeOffset.UtcNow)
            .Select(su => su.User)
            .FirstOrDefaultAsync();
        
        return user;
    }

    public async Task<User?> GetPremiumUser()
    {
        var user = await GetAuthenticatedUser();
        
        if (user?.Role == UserRole.Admin)
        {
            return user;
        }
        
        return user?.Subscription is not null && user.Subscription.Expires > DateTimeOffset.UtcNow ? user : null;
    }
    
    private Guid? GetSessionUserId()
    {
        var userSessionId = _authDataProvider.GetSessionUserId();

        return userSessionId ?? null;
    }
}