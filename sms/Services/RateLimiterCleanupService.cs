using SMSGateway.Models;
using static SMSGateway.Models.Result;

public class RateLimiterCleanupService : BackgroundService
{
    private readonly SmsRateLimiter _rateLimiter;

    public RateLimiterCleanupService(SmsRateLimiter rateLimiter)
    {
        _rateLimiter = rateLimiter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _rateLimiter.CleanupOldEntries();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}