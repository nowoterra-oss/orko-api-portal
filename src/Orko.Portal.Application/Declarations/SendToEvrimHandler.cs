using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
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

        // JSON'dan Evrim Declaration modeline donustur
        EvrimDeclarationRequest evrimRequest;
        try
        {
            evrimRequest = JsonSerializer.Deserialize<EvrimDeclarationRequest>(
                declaration.DeclarationData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new EvrimDeclarationRequest();
        }
        catch (JsonException)
        {
            // Eski format ise wrapper olarak gonder
            evrimRequest = new EvrimDeclarationRequest
            {
                ReferansNo = declaration.WorkOrder.FileNumber,
                Aciklamalar = declaration.DeclarationData
            };
        }

        // Referans numarasini her zaman set et
        evrimRequest.ReferansNo ??= declaration.WorkOrder.FileNumber;
        evrimRequest.Ihracat = declaration.DeclarationType == DeclarationType.Export;

        // Evrim'e gonder (tip'e gore import/export)
        EvrimResponse result;
        if (declaration.DeclarationType == DeclarationType.Export)
        {
            result = await _evrim.CreateExportDeclarationAsync(evrimRequest);
        }
        else
        {
            result = await _evrim.CreateImportDeclarationAsync(evrimRequest);
        }

        // Sonucu kaydet
        declaration.SentToEvrim = result.Success;
        declaration.SentAt = DateTime.UtcNow;
        declaration.EvrimResponse = result.RawResponse;
        declaration.EvrimDeclarationId = result.EvrimReferansNo;

        if (result.Success)
        {
            declaration.Status = WorkOrderStatus.EvrimeGonderildi;
            declaration.WorkOrder.Status = WorkOrderStatus.EvrimeGonderildi;
            declaration.WorkOrder.UpdatedAt = DateTime.UtcNow;

            _db.StatusHistories.Add(new Domain.Entities.StatusHistory
            {
                Id = Guid.NewGuid(),
                WorkOrderId = declaration.WorkOrderId,
                FromStatus = WorkOrderStatus.Hazirlaniyor,
                ToStatus = WorkOrderStatus.EvrimeGonderildi,
                ChangedBy = "System",
                Note = $"Evrim'e basariyla gonderildi. EvrimRef: {result.EvrimReferansNo}",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Evrim gonderim: {FileNumber} | Basarili: {Success} | EvrimRef: {Ref} | Mesaj: {Msg}",
            declaration.WorkOrder.FileNumber, result.Success, result.EvrimReferansNo, result.ExceptionMessage);

        return result;
    }
}
