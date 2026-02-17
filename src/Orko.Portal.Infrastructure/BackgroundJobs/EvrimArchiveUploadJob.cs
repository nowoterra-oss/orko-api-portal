using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Infrastructure.BackgroundJobs;

public class EvrimArchiveUploadJob
{
    private readonly PortalDbContext _db;
    private readonly IEvrimApiClient _evrim;
    private readonly ILogger<EvrimArchiveUploadJob> _logger;

    public EvrimArchiveUploadJob(
        PortalDbContext db, IEvrimApiClient evrim, ILogger<EvrimArchiveUploadJob> logger)
    {
        _db = db;
        _evrim = evrim;
        _logger = logger;
    }

    [Hangfire.AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 })]
    public async Task ExecuteAsync(Guid archiveId)
    {
        var archive = await _db.Archives
            .Include(a => a.Declaration)
                .ThenInclude(d => d.WorkOrder)
            .FirstOrDefaultAsync(a => a.Id == archiveId);

        if (archive == null) return;
        if (string.IsNullOrEmpty(archive.Declaration?.EvrimDeclarationId)) return;

        var request = new EvrimArchiveRequest
        {
            FileName = archive.FileName,
            FileRefNo = archive.Declaration.WorkOrder.FileNumber,
            FileData = archive.Base64Data,
            BelgeKod = archive.DocumentType?[..Math.Min(4, archive.DocumentType?.Length ?? 0)],
            Tarih = archive.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
            Aciklama = archive.FileName,
            Referans = archive.Declaration.EvrimDeclarationId,
            EklemeTipi = "F"  // Firma tarafindan ekleniyor
        };

        var result = await _evrim.SendWorkOrderArchiveAsync(request);

        archive.SentToEvrim = result.Success;
        archive.SentAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Evrim arsiv yukleme: {FileName} -> {FileNumber} | Basarili: {Success}",
            archive.FileName, archive.Declaration.WorkOrder.FileNumber, result.Success);
    }
}
