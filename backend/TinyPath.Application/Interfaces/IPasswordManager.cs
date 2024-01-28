namespace TinyPath.Application.Interfaces;

public interface IPasswordManager
{
    string HashPassword(string password);
    bool VerifyPassword(string passwordHash, string password);
}