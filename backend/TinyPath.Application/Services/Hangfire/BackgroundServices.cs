using Hangfire;
using Microsoft.Extensions.Logging;
using TinyPath.Application.Services.Guest;
using TinyPath.Application.Services.User;

namespace TinyPath.Application.Services.Hangfire;

public class BackgroundServices : IBackgroundServices
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IGuestManager _guestManager;
    private readonly ILogger<BackgroundServices> _logger;
    private readonly IUserManager _userManager;

    public BackgroundServices(IBackgroundJobClient backgroundJobClient, IGuestManager guestManager, ILogger<BackgroundServices> logger, IUserManager userManager)
    {
        _backgroundJobClient = backgroundJobClient;
        _guestManager = guestManager;
        _logger = logger;
        _userManager = userManager;
    }

    public void UnblockGuestUser(Guid guestId, int delayInMinutes)
    {
        _backgroundJobClient.Schedule(() => _guestManager.UnblockGuestUser(guestId), TimeSpan.FromMinutes(delayInMinutes));
        _logger.LogInformation($"Unblocking guest user with id: {guestId} in {delayInMinutes} minutes.");
    }
    
    public string DeleteExpiredSessions(Guid userId, int delayInMinutes)
    {
        var jobId = _backgroundJobClient.Schedule(() => _userManager.DeleteExpiredSessions(userId), TimeSpan.FromMinutes(delayInMinutes));
        _logger.LogInformation("Deleting expired sessions.");
        
        return jobId;
    }

    public void DeleteScheduledJob(string jobId)
    {
        _backgroundJobClient.Delete(jobId);
        _logger.LogInformation($"Deleting job with id: {jobId}.");
    }
    
    public void RescheduleJob(string jobId, int delayInMinutes)
    {
        _backgroundJobClient.Reschedule(jobId, TimeSpan.FromMinutes(delayInMinutes));
        _logger.LogInformation($"Rescheduling job with id: {jobId}.");
    }
}