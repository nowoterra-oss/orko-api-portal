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
        PortalDbContext db, IEvrimApiClient evrim, ILogger<EvrimStatusNotificationJob> logger)
    {
        _db = db;
        _evrim = evrim;
        _logger = logger;
    }

    [Hangfire.AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 })]
    public async Task ExecuteAsync(Guid statusHistoryId)
    {
        var history = await _db.StatusHistories
            .Include(s => s.WorkOrder)
                .ThenInclude(w => w.Declaration)
            .FirstOrDefaultAsync(s => s.Id == statusHistoryId);

        if (history == null) return;

        var declaration = history.WorkOrder.Declaration;
        if (declaration == null || string.IsNullOrEmpty(declaration.EvrimDeclarationId)) return;

        var request = new EvrimStatusRequest
        {
            EvrakTip = 0,  // Beyanname
            ReferansNo = history.WorkOrder.FileNumber,
            DosyaNo = declaration.EvrimDeclarationId,
            IsTakipler = new List<EvrimIsTakip>
            {
                new()
                {
                    Kod = history.ToStatus.ToString()[..Math.Min(3, history.ToStatus.ToString().Length)],
                    TarihSaat = history.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Aciklama = $"{history.FromStatus} -> {history.ToStatus}",
                    IsNot = history.Note
                }
            }
        };

        var result = await _evrim.CreateStatusAsync(request);

        history.EvrimNotified = result.Success;
        history.EvrimNotifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Evrim statu bildirimi: {FileNumber} | {Status} | Basarili: {Success}",
            history.WorkOrder.FileNumber, history.ToStatus, result.Success);
    }
}
