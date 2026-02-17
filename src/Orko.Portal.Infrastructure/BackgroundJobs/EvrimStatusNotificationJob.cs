using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Infrastructure.BackgroundJobs;

public class EvrimStatusNotificationJob
{
    private readonly PortalDbContext _db;
    private readonly IEvrimApiClient _evrim;
    private readonly ILogger<EvrimStatusNotificationJob> _logger;

    public EvrimStatusNotificationJob(
        PortalDbContext db,
        IEvrimApiClient evrim,
        ILogger<EvrimStatusNotificationJob> logger)
    {
        _db = db;
        _evrim = evrim;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new int[] { 10, 30, 60 })]
    public async Task ExecuteAsync(Guid statusHistoryId)
    {
        var history = await _db.StatusHistories
            .Include(h => h.WorkOrder)
                .ThenInclude(w => w.Declaration)
            .FirstOrDefaultAsync(h => h.Id == statusHistoryId);

        if (history == null)
        {
            _logger.LogWarning("StatusHistory bulunamadi: {Id}", statusHistoryId);
            return;
        }

        if (history.EvrimNotified)
        {
            _logger.LogInformation("StatusHistory zaten bildirilmis: {Id}", statusHistoryId);
            return;
        }

        var evrimDeclarationId = history.WorkOrder?.Declaration?.EvrimDeclarationId;
        if (string.IsNullOrEmpty(evrimDeclarationId))
        {
            _logger.LogWarning(
                "Evrim DeclarationId bulunamadi, statu bildirimi yapilamadi: {FileNumber}",
                history.WorkOrder?.FileNumber);
            return;
        }

        var request = new EvrimStatusRequest
        {
            DeclarationId = evrimDeclarationId,
            Status = history.ToStatus.ToString()
        };

        var result = await _evrim.UpdateStatusAsync(request);

        history.EvrimNotified = result.Success;
        history.EvrimNotifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        if (result.Success)
        {
            _logger.LogInformation(
                "Evrim statu bildirimi basarili: {FileNumber} -> {Status}",
                history.WorkOrder?.FileNumber, history.ToStatus);
        }
        else
        {
            _logger.LogError(
                "Evrim statu bildirimi basarisiz: {FileNumber} -> {Status} | Hata: {Error}",
                history.WorkOrder?.FileNumber, history.ToStatus, result.Message);
            throw new Exception($"Evrim statu bildirimi basarisiz: {result.Message}");
        }
    }
}
