namespace MiniMercadoInteligente.Domain.Entities;

public class FraudEventLock
{
    public Guid FraudEventLockId { get; set; }
    public string Source { get; set; } = default!;
    public string ExternalEventId { get; set; } = default!;
    public string Hash { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}