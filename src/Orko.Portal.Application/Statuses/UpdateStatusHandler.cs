using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Contracts.Statuses;
using Orko.Portal.Domain;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Infrastructure.BackgroundJobs;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Statuses;

public class UpdateStatusHandler
{
    private readonly PortalDbContext _db;
    private readonly IBackgroundJobClient _jobs;
    private readonly ILogger<UpdateStatusHandler> _logger;

    public UpdateStatusHandler(
        PortalDbContext db,
        IBackgroundJobClient jobs,
        ILogger<UpdateStatusHandler> logger)
    {
        _db = db;
        _jobs = jobs;
        _logger = logger;
    }

    public async Task<StatusHistoryDto> HandleAsync(
        Guid declarationId, UpdateStatusDto dto, string? changedBy = null)
    {
        var declaration = await _db.Declarations
            .Include(d => d.WorkOrder)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (declaration == null)
            throw new KeyNotFoundException("Beyanname bulunamadi.");

        // Yeni statu parse
        if (!Enum.TryParse<WorkOrderStatus>(dto.NewStatus, true, out var newStatus))
            throw new ArgumentException($"Gecersiz statu: {dto.NewStatus}");

        var currentStatus = declaration.WorkOrder.Status;

        // Gecis kontrolu
        StatusMachine.EnsureValidTransition(currentStatus, newStatus);

        // Guncelle
        var now = DateTime.UtcNow;
        declaration.WorkOrder.Status = newStatus;
        declaration.WorkOrder.UpdatedAt = now;
        declaration.Status = newStatus;
        declaration.UpdatedAt = now;
        declaration.UpdatedBy = changedBy;

        // Gecmis kaydi
        var history = new StatusHistory
        {
            Id = Guid.NewGuid(),
            WorkOrderId = declaration.WorkOrderId,
            FromStatus = currentStatus,
            ToStatus = newStatus,
            ChangedBy = changedBy,
            Note = dto.Note,
            CreatedAt = now
        };

        _db.StatusHistories.Add(history);
        await _db.SaveChangesAsync();

        // Evrim'e bildirim (background job)
        if (declaration.SentToEvrim && !string.IsNullOrEmpty(declaration.EvrimDeclarationId))
        {
            _jobs.Enqueue<EvrimStatusNotificationJob>(
                job => job.ExecuteAsync(history.Id));
        }

        _logger.LogInformation(
            "Statu guncellendi: {FileNumber} | {From} -> {To} | Kullanici: {User}",
            declaration.WorkOrder.FileNumber, currentStatus, newStatus, changedBy);

        return new StatusHistoryDto
        {
            Id = history.Id,
            FromStatus = currentStatus.ToString(),
            ToStatus = newStatus.ToString(),
            ChangedBy = changedBy,
            Note = dto.Note,
            EvrimNotified = history.EvrimNotified,
            CreatedAt = history.CreatedAt
        };
    }

    public async Task<AllowedTransitionsDto?> GetAllowedTransitionsAsync(Guid declarationId)
    {
        var declaration = await _db.Declarations
            .Include(d => d.WorkOrder)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (declaration == null) return null;

        var currentStatus = declaration.WorkOrder.Status;
        var allowed = StatusMachine.GetAllowedTransitions(currentStatus);

        return new AllowedTransitionsDto
        {
            CurrentStatus = currentStatus.ToString(),
            AllowedTransitions = allowed.Select(s => s.ToString()).ToList()
        };
    }
}
