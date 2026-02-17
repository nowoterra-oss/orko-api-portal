using Orko.Portal.Domain.Enums;

namespace Orko.Portal.Domain;

public static class StatusMachine
{
    private static readonly Dictionary<WorkOrderStatus, WorkOrderStatus[]> Transitions = new()
    {
        [WorkOrderStatus.Taslak]            = [WorkOrderStatus.Hazirlaniyor],
        [WorkOrderStatus.Hazirlaniyor]      = [WorkOrderStatus.EvrimeGonderildi],
        [WorkOrderStatus.EvrimeGonderildi]  = [WorkOrderStatus.TescilBekleniyor,
                                                WorkOrderStatus.RedEdildi,
                                                WorkOrderStatus.DuzeltmeGerekli],
        [WorkOrderStatus.TescilBekleniyor]  = [WorkOrderStatus.TescilEdildi,
                                                WorkOrderStatus.RedEdildi],
        [WorkOrderStatus.DuzeltmeGerekli]   = [WorkOrderStatus.Hazirlaniyor],
        [WorkOrderStatus.TescilEdildi]      = [WorkOrderStatus.Kapandi],
    };

    public static bool CanTransition(WorkOrderStatus from, WorkOrderStatus to)
        => Transitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public static WorkOrderStatus[] GetAllowedTransitions(WorkOrderStatus from)
        => Transitions.TryGetValue(from, out var allowed) ? allowed : [];

    public static void EnsureValidTransition(WorkOrderStatus from, WorkOrderStatus to)
    {
        if (!CanTransition(from, to))
            throw new InvalidOperationException(
                $"Gecersiz statu gecisi: {from} -> {to}");
    }
}
