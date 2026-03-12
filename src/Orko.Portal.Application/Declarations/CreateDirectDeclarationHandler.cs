using System.Text.Json;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Infrastructure.BackgroundJobs;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Declarations;

/// <summary>
/// Disaridan gelen is emri verisini alir, WorkOrder + Declaration olusturur.
/// Evrim'e gonderme ayri adimda yapilir (declarations/{id}/send).
/// </summary>
public class CreateDirectDeclarationHandler
{
    private readonly PortalDbContext _db;
    private readonly IBackgroundJobClient _jobs;
    private readonly ILogger<CreateDirectDeclarationHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public CreateDirectDeclarationHandler(
        PortalDbContext db,
        IBackgroundJobClient jobs,
        ILogger<CreateDirectDeclarationHandler> logger)
    {
        _db = db;
        _jobs = jobs;
        _logger = logger;
    }

    public record DirectDeclarationResult(string FileNumber, Guid WorkOrderId, Guid DeclarationId);

    public async Task<DirectDeclarationResult> HandleExportAsync(EvrimExportDeclarationRequest request)
    {
        return await CreateWorkOrderAndDeclaration(request, DeclarationType.Export);
    }

    public async Task<DirectDeclarationResult> HandleImportAsync(EvrimCreateDeclarationRequest request)
    {
        return await CreateWorkOrderAndDeclaration(request, DeclarationType.Import);
    }

    private async Task<DirectDeclarationResult> CreateWorkOrderAndDeclaration(
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
            DeclarationData = JsonSerializer.Serialize(request, request.GetType(), JsonOptions),
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

        return new DirectDeclarationResult(fileNumber, workOrder.Id, declaration.Id);
    }
}
