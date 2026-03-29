namespace MiniMercadoInteligente.Domain.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}