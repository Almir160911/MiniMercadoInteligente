namespace MiniMercadoInteligente.Domain.Entities;

public enum AlertSeverity
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum AlertStatus
{
    Open = 0,
    Resolved = 1,
    Dismissed = 2
}

public class Alert
{
    public Guid AlertId { get; set; }
    public Guid? SessionId { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Type { get; set; } = default!;
    public AlertStatus Status { get; set; } = AlertStatus.Open;
    public decimal EstimatedLoss { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public string PayloadJson { get; set; } = "{}";
}