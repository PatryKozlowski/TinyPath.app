namespace TinyPath.Application.Services.User;

public interface IUserManager
{
    Task DeleteExpiredSessions(Guid userId);
}