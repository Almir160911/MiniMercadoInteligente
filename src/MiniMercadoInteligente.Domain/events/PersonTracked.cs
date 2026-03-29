namespace MiniMercadoInteligente.Domain.Events;

public sealed class PersonTracked : DomainEvent
{
    public Guid? SessionId { get; init; }
    public string CameraId { get; init; } = string.Empty;
    public string TrackId { get; init; } = string.Empty;
    public string AreaCode { get; init; } = string.Empty;
    public decimal X { get; init; }
    public decimal Y { get; init; }
}