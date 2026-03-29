namespace MiniMercadoInteligente.Domain.Events;

public sealed class CustomerExitedStore : DomainEvent
{
    public Guid SessionId { get; init; }
    public Guid ResidentId { get; init; }
    public string GateId { get; init; } = string.Empty;
}