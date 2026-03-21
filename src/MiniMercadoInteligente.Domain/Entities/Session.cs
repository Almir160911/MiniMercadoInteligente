namespace MiniMercadoInteligente.Domain.Entities;

public enum SessionStatus
{
    Open = 0,
    Paid = 1,
    Closed = 2,
    Cancelled = 3,
    Blocked = 4
}

public class Session
{
    public Guid SessionId { get; set; }
    public Guid ResidentId { get; set; }
    public Guid CondominiumId { get; set; }
    public string DeviceId { get; set; } = default!;
    public SessionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    // antifraude
    public bool FraudSuspected { get; set; } = false;
    public int FraudScore { get; set; } = 0;
    public string FraudFlagsJson { get; set; } = "[]";
}