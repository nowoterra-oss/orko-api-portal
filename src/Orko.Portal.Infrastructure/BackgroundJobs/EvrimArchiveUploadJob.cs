using Hangfire;
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
        PortalDbContext db,
        IEvrimApiClient evrim,
        ILogger<EvrimArchiveUploadJob> logger)
    {
        _db = db;
        _evrim = evrim;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new int[] { 10, 30, 60 })]
    public async Task ExecuteAsync(Guid archiveId)
    {
        var archive = await _db.Archives
            .Include(a => a.Declaration)
            .FirstOrDefaultAsync(a => a.Id == archiveId);

        if (archive == null)
        {
            _logger.LogWarning("Arsiv bulunamadi: {Id}", archiveId);
            return;
        }

        if (archive.SentToEvrim)
        {
            _logger.LogInformation("Arsiv zaten gonderilmis: {Id}", archiveId);
            return;
        }

        var evrimDeclarationId = archive.Declaration?.EvrimDeclarationId;
        if (string.IsNullOrEmpty(evrimDeclarationId))
        {
            _logger.LogWarning("Evrim DeclarationId yok, arsiv gonderilemedi: {FileName}", archive.FileName);
            return;
        }

        var request = new EvrimArchiveRequest
        {
            DeclarationId = evrimDeclarationId,
            FileName = archive.FileName,
            Base64Data = archive.Base64Data,
            DocumentType = archive.DocumentType
        };

        var result = await _evrim.UploadArchiveAsync(request);

        archive.SentToEvrim = result.Success;
        archive.SentAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        if (result.Success)
        {
            _logger.LogInformation("Arsiv Evrim'e gonderildi: {FileName}", archive.FileName);
        }
        else
        {
            _logger.LogError("Arsiv gonderilemedi: {FileName} | Hata: {Error}", archive.FileName, result.Message);
            throw new Exception($"Arsiv gonderilemedi: {result.Message}");
        }
    }
}
