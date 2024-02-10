using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Interfaces;

namespace TinyPath.Application.Services.User;

public class UserManager : IUserManager
{
    private readonly IApplicationDbContext _dbContext;

    public UserManager(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteExpiredSessions(Guid userId)
    {
        var userSession = await _dbContext
            .Sessions
            .FirstOrDefaultAsync(s => s.Id == userId);
        
        _dbContext.Sessions.Remove(userSession!);
        
        await _dbContext.SaveChangesAsync();
    }
}