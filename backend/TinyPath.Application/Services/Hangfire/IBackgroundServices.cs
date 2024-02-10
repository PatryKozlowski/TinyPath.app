namespace TinyPath.Application.Services.Hangfire;

public interface IBackgroundServices
{
    void UnblockGuestUser(Guid guestId, int delayInMinutes);
    string DeleteExpiredSessions(Guid userId, int delayInMinutes);
    void DeleteScheduledJob(string jobId);
    void RescheduleJob(string jobId, int delayInMinutes);
}