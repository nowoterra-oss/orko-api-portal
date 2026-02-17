using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Contracts.Archives;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Infrastructure.BackgroundJobs;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Archives;

public class UploadArchiveHandler
{
    private readonly PortalDbContext _db;
    private readonly IBackgroundJobClient _jobs;
    private readonly ILogger<UploadArchiveHandler> _logger;

    public UploadArchiveHandler(
        PortalDbContext db,
        IBackgroundJobClient jobs,
        ILogger<UploadArchiveHandler> logger)
    {
        _db = db;
        _jobs = jobs;
        _logger = logger;
    }

    public async Task<ArchiveDto> HandleAsync(
        Guid declarationId, UploadArchiveDto dto, string? uploadedBy = null)
    {
        var declaration = await _db.Declarations
            .Include(d => d.WorkOrder)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (declaration == null)
            throw new KeyNotFoundException("Beyanname bulunamadi.");

        // Base64 boyut hesapla
        var fileSize = (long)(dto.Base64Data.Length * 0.75);

        var archive = new Archive
        {
            Id = Guid.NewGuid(),
            DeclarationId = declarationId,
            FileName = dto.FileName,
            FileSize = fileSize,
            ContentType = dto.ContentType,
            DocumentType = dto.DocumentType,
            Base64Data = dto.Base64Data,
            UploadedBy = uploadedBy,
            CreatedAt = DateTime.UtcNow
        };

        _db.Archives.Add(archive);
        await _db.SaveChangesAsync();

        // Evrim'e gonderilmis beyannameye ait arsiv ise otomatik gonder
        if (declaration.SentToEvrim && !string.IsNullOrEmpty(declaration.EvrimDeclarationId))
        {
            _jobs.Enqueue<EvrimArchiveUploadJob>(
                job => job.ExecuteAsync(archive.Id));
        }

        _logger.LogInformation(
            "Arsiv yuklendi: {FileName} ({Size} byte) -> {FileNumber}",
            dto.FileName, fileSize, declaration.WorkOrder.FileNumber);

        return new ArchiveDto
        {
            Id = archive.Id,
            FileName = archive.FileName,
            FileSize = archive.FileSize,
            ContentType = archive.ContentType,
            DocumentType = archive.DocumentType,
            SentToEvrim = archive.SentToEvrim,
            UploadedBy = uploadedBy,
            CreatedAt = archive.CreatedAt
        };
    }

    public async Task<List<ArchiveDto>> GetByDeclarationAsync(Guid declarationId)
    {
        return await _db.Archives
            .Where(a => a.DeclarationId == declarationId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ArchiveDto
            {
                Id = a.Id,
                FileName = a.FileName,
                FileSize = a.FileSize,
                ContentType = a.ContentType,
                DocumentType = a.DocumentType,
                SentToEvrim = a.SentToEvrim,
                SentAt = a.SentAt,
                UploadedBy = a.UploadedBy,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> SendToEvrimAsync(Guid archiveId)
    {
        var archive = await _db.Archives
            .Include(a => a.Declaration)
            .FirstOrDefaultAsync(a => a.Id == archiveId);

        if (archive == null) return false;

        if (archive.SentToEvrim)
            throw new InvalidOperationException("Arsiv zaten Evrim'e gonderilmis.");

        if (string.IsNullOrEmpty(archive.Declaration?.EvrimDeclarationId))
            throw new InvalidOperationException("Beyanname henuz Evrim'e gonderilmemis.");

        _jobs.Enqueue<EvrimArchiveUploadJob>(
            job => job.ExecuteAsync(archive.Id));

        return true;
    }
}
