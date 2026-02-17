using Microsoft.EntityFrameworkCore;
using Orko.Portal.Contracts.Archives;
using Orko.Portal.Contracts.Declarations;
using Orko.Portal.Contracts.Statuses;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Declarations;

public class GetDeclarationHandler
{
    private readonly PortalDbContext _db;

    public GetDeclarationHandler(PortalDbContext db) => _db = db;

    public async Task<DeclarationDetailDto?> HandleAsync(Guid declarationId)
    {
        var d = await _db.Declarations
            .Include(d => d.WorkOrder)
            .Include(d => d.Archives)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (d == null) return null;

        var statusHistory = await _db.StatusHistories
            .Where(s => s.WorkOrderId == d.WorkOrderId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new StatusHistoryDto
            {
                Id = s.Id,
                FromStatus = s.FromStatus.ToString(),
                ToStatus = s.ToStatus.ToString(),
                ChangedBy = s.ChangedBy,
                Note = s.Note,
                EvrimNotified = s.EvrimNotified,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return new DeclarationDetailDto
        {
            Id = d.Id,
            WorkOrderId = d.WorkOrderId,
            FileNumber = d.WorkOrder.FileNumber,
            DeclarationType = d.DeclarationType.ToString(),
            Status = d.Status.ToString(),
            DeclarationData = d.DeclarationData,
            EvrimDeclarationId = d.EvrimDeclarationId,
            EvrimResponse = d.EvrimResponse,
            SentToEvrim = d.SentToEvrim,
            SentAt = d.SentAt,
            UpdatedBy = d.UpdatedBy,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            Archives = d.Archives.Select(a => new ArchiveDto
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
            }).ToList(),
            StatusHistory = statusHistory
        };
    }
}
