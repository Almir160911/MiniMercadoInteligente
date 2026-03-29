namespace MiniMercadoInteligente.Domain.Events;

public sealed class ProductReturned : DomainEvent
{
    public Guid SessionId { get; init; }
    public Guid ResidentId { get; init; }
    public Guid ProductId { get; init; }
    public string AreaCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string? EvidenceId { get; init; } = string.Empty;
}