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
/// Export icin EvrimExportDeclarationRequest, Import icin EvrimCreateDeclarationRequest kullanir.
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

    public async Task<EvrimResponse> HandleExportAsync(EvrimExportDeclarationRequest request)
    {
        var (workOrder, declaration) = await CreateWorkOrderAndDeclaration(
            request, DeclarationType.Export);

        var result = await _evrim.CreateNewExportDeclarationAsync(request);

        await SaveEvrimResult(workOrder, declaration, result);
        return result;
    }

    public async Task<EvrimResponse> HandleImportAsync(EvrimCreateDeclarationRequest request)
    {
        var (workOrder, declaration) = await CreateWorkOrderAndDeclaration(
            request, DeclarationType.Import);

        var result = await _evrim.CreateNewImportDeclarationAsync(request);

        await SaveEvrimResult(workOrder, declaration, result);
        return result;
    }

    private async Task<(WorkOrder workOrder, Declaration declaration)> CreateWorkOrderAndDeclaration(
        object request, DeclarationType type)
    {
        var today = DateTime.UtcNow;
        var count = await _db.WorkOrders
            .CountAsync(w => w.CreatedAt.Date == today.Date);
        var fileNumber = $"ORK-{today:yyyyMMdd}-{(count + 1):D4}";

        var workOrder = new WorkOrder
        {
            Id = Guid.NewGuid(),
            FileNumber = fileNumber,
            Type = type,
            Status = WorkOrderStatus.Hazirlaniyor,
            CreatedAt = today,
            UpdatedAt = today
        };

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

        _jobs.Enqueue<WorkOrderEmailNotificationJob>(job => job.ExecuteAsync(workOrder.Id));

        return (workOrder, declaration);
    }

    private async Task SaveEvrimResult(WorkOrder workOrder, Declaration declaration, EvrimResponse result)
    {
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
            workOrder.FileNumber, result.Success, result.EvrimReferansNo);
    }
}
