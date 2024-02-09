namespace TinyPath.Application.Services.Hangfire;

public interface IBackgroundServices
{
    void UnblockGuestUser(Guid guestId, int delayInMinutes);
}