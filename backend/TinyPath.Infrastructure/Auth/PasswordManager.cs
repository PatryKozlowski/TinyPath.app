using Microsoft.AspNetCore.Identity;
using TinyPath.Application.Interfaces;

namespace TinyPath.Infrastructure.Auth;

public class PasswordManager : IPasswordManager
{
    private readonly IPasswordHasher<DummyUser> _passwordHasher;

    public PasswordManager(IPasswordHasher<DummyUser> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new DummyUser(), password);
    }

    public bool VerifyPassword(string passwordHash, string password)
    {
        var verifyResult = _passwordHasher.VerifyHashedPassword(new DummyUser(), passwordHash, password);
        
        return verifyResult == PasswordVerificationResult.Success
               || verifyResult == PasswordVerificationResult.SuccessRehashNeeded;
    }
    
    public class DummyUser() {}
}