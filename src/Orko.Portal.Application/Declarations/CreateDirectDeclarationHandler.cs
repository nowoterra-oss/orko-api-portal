using System.Text.Json;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.BackgroundJobs;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Declarations;

/// <summary>
/// Declaration body'sini alir, WorkOrder + Declaration olusturur, Evrim'e gonderir.
/// Evrim'in create_export_declaration / create_import_declaration endpoint'lerinin aynisi.
/// Yeni Evrim semasini (EvrimCreateDeclarationRequest) kullanir.
/// </summary>
public class CreateDirectDeclarationHandler
{
    private readonly PortalDbContext _db;
    private readonly IEvrimApiClient _evrim;
    private readonly IBackgroundJobClient _jobs;
    private readonly ILogger<CreateDirectDeclarationHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public CreateDirectDeclarationHandler(
        PortalDbContext db,
        IEvrimApiClient evrim,
        IBackgroundJobClient jobs,
        ILogger<CreateDirectDeclarationHandler> logger)
    {
        _db = db;
        _evrim = evrim;
        _jobs = jobs;
        _logger = logger;
    }

    public async Task<EvrimResponse> HandleAsync(EvrimCreateDeclarationRequest request, DeclarationType type)
    {
        // Dosya numarasi uret
        var today = DateTime.UtcNow;
        var count = await _db.WorkOrders
            .CountAsync(w => w.CreatedAt.Date == today.Date);
        var fileNumber = $"ORK-{today:yyyyMMdd}-{(count + 1):D4}";

        // Is emri olustur
        var workOrder = new WorkOrder
        {
            Id = Guid.NewGuid(),
            FileNumber = fileNumber,
            Type = type,
            Status = WorkOrderStatus.Hazirlaniyor,
            CreatedAt = today,
            UpdatedAt = today
        };

        // Beyanname olustur ve veriyi kaydet
        var declaration = new Declaration
        {
            Id = Guid.NewGuid(),
            WorkOrderId = workOrder.Id,
            DeclarationType = type,
            DeclarationData = JsonSerializer.Serialize(request, JsonOptions),
            Status = WorkOrderStatus.Hazirlaniyor,
            CreatedAt = today,
            UpdatedAt = today
        };

        _db.WorkOrders.Add(workOrder);
        _db.Declarations.Add(declaration);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Direct declaration olusturuldu: {FileNumber} | Tip: {Type}",
            fileNumber, type);

        // E-posta bildirimi (arka planda)
        _jobs.Enqueue<WorkOrderEmailNotificationJob>(job => job.ExecuteAsync(workOrder.Id));

        // Evrim'e gonder (yeni endpoint'ler)
        EvrimResponse result;
        if (type == DeclarationType.Export)
            result = await _evrim.CreateNewExportDeclarationAsync(request);
        else
            result = await _evrim.CreateNewImportDeclarationAsync(request);

        // Sonucu kaydet
        declaration.SentToEvrim = result.Success;
        declaration.SentAt = DateTime.UtcNow;
        declaration.EvrimResponse = result.RawResponse;
        declaration.EvrimDeclarationId = result.EvrimReferansNo;

        if (result.Success)
        {
            declaration.Status = WorkOrderStatus.EvrimeGonderildi;
            workOrder.Status = WorkOrderStatus.EvrimeGonderildi;
            workOrder.UpdatedAt = DateTime.UtcNow;

            _db.StatusHistories.Add(new StatusHistory
            {
                Id = Guid.NewGuid(),
                WorkOrderId = workOrder.Id,
                FromStatus = WorkOrderStatus.Hazirlaniyor,
                ToStatus = WorkOrderStatus.EvrimeGonderildi,
                ChangedBy = "System",
                Note = $"Evrim'e basariyla gonderildi. EvrimRef: {result.EvrimReferansNo}",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Direct declaration Evrim sonuc: {FileNumber} | Basarili: {Success} | Ref: {Ref}",
            fileNumber, result.Success, result.EvrimReferansNo);

        return result;
    }
}
