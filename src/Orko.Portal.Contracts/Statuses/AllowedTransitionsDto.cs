namespace Orko.Portal.Contracts.Statuses;

public class AllowedTransitionsDto
{
    public string CurrentStatus { get; set; } = default!;
    public List<string> AllowedTransitions { get; set; } = new();
}
