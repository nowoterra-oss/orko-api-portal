using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Declarations;

public class SendToEvrimHandler
{
    private readonly PortalDbContext _db;
    private readonly IEvrimApiClient _evrim;
    private readonly ILogger<SendToEvrimHandler> _logger;

    public SendToEvrimHandler(PortalDbContext db, IEvrimApiClient evrim, ILogger<SendToEvrimHandler> logger)
    {
        _db = db;
        _evrim = evrim;
        _logger = logger;
    }

    public async Task<EvrimResponse> HandleAsync(Guid declarationId)
    {
        var declaration = await _db.Declarations
            .Include(d => d.WorkOrder)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (declaration == null)
            throw new KeyNotFoundException("Beyanname bulunamadi.");

        if (declaration.SentToEvrim)
            throw new InvalidOperationException("Beyanname zaten Evrim'e gonderilmis.");

        if (string.IsNullOrEmpty(declaration.DeclarationData))
            throw new InvalidOperationException("Beyanname verileri bos. Once formu doldurun.");

        // Evrim'e gonder (tip'e gore import/export)
        EvrimResponse result;
        if (declaration.DeclarationType == DeclarationType.Export)
        {
            var request = new EvrimExportRequest { DeclarationData = declaration.DeclarationData };
            result = await _evrim.CreateExportDeclarationAsync(request);
        }
        else
        {
            var request = new EvrimImportRequest { DeclarationData = declaration.DeclarationData };
            result = await _evrim.CreateImportDeclarationAsync(request);
        }

        // Sonucu kaydet
        declaration.SentToEvrim = result.Success;
        declaration.SentAt = DateTime.UtcNow;
        declaration.EvrimResponse = result.RawResponse;
        declaration.EvrimDeclarationId = result.DeclarationId;

        if (result.Success)
        {
            declaration.Status = WorkOrderStatus.EvrimeGonderildi;
            declaration.WorkOrder.Status = WorkOrderStatus.EvrimeGonderildi;
            declaration.WorkOrder.UpdatedAt = DateTime.UtcNow;

            // Statu gecmisi
            _db.StatusHistories.Add(new Domain.Entities.StatusHistory
            {
                Id = Guid.NewGuid(),
                WorkOrderId = declaration.WorkOrderId,
                FromStatus = WorkOrderStatus.Hazirlaniyor,
                ToStatus = WorkOrderStatus.EvrimeGonderildi,
                ChangedBy = "System",
                Note = "Evrim'e basariyla gonderildi.",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Evrim gonderim sonucu: {FileNumber} | Basarili: {Success} | EvrimId: {EvrimId}",
            declaration.WorkOrder.FileNumber, result.Success, result.DeclarationId);

        return result;
    }
}
