using System.Security.Cryptography;
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
    
    public int GenerateDeleteCode()
    {
        using var rng = new RNGCryptoServiceProvider();
        var randomNumber = new byte[4];
        rng.GetBytes(randomNumber);
        var value = BitConverter.ToInt32(randomNumber, 0) & int.MaxValue;
        return 100000 + value % 900000;
    }
}