namespace MiniMercadoInteligente.Domain.Events;

public sealed class CustomerEnteredStore : DomainEvent
{
    public Guid SessionId { get; init; }
    public Guid ResidentId { get; init; }
    public string GateId { get; init; } = string.Empty;
}