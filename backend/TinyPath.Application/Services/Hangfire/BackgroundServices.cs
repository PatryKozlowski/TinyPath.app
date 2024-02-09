using Hangfire;
using Hangfire.Common;
using Microsoft.Extensions.Logging;
using TinyPath.Application.Services.Guest;

namespace TinyPath.Application.Services.Hangfire;

public class BackgroundServices : IBackgroundServices
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IGuestManager _guestManager;
    private readonly ILogger<BackgroundServices> _logger;

    public BackgroundServices(IBackgroundJobClient backgroundJobClient, IGuestManager guestManager, ILogger<BackgroundServices> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _guestManager = guestManager;
        _logger = logger;
    }

    public void UnblockGuestUser(Guid guestId, int delayInMinutes)
    {
        _backgroundJobClient.Schedule(() => _guestManager.UnblockGuestUser(guestId), TimeSpan.FromMinutes(delayInMinutes));
        _logger.LogInformation($"Unblocking guest user with id: {guestId} in {delayInMinutes} minutes.");
    }
}