namespace MiniMercadoInteligente.Domain.Events;

public sealed class WeightChangedDetected : DomainEvent
{
    public string SensorId { get; init; } = string.Empty;
    public string AreaCode { get; init; } = string.Empty;
    public decimal PreviousWeightGrams { get; init; }
    public decimal CurrentWeightGrams { get; init; }
}