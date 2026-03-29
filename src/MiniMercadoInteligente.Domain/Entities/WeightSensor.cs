namespace MiniMercadoInteligente.Domain.Entities;

public class WeightSensor
{
    public Guid Id { get; set; }

    public string SensorId { get; set; } = default!;
    public string Name { get; set; } = default!;

    public Guid CondominiumId { get; set; }

    public Guid? AreaId { get; set; }
    public string AreaCode { get; set; } = default!;

    public decimal CurrentWeightGrams { get; set; }
    public decimal LastWeightGrams { get; set; }

    public decimal ToleranceGrams { get; set; } = 50m;

    public bool Active { get; set; } = true;

    public DateTime LastReadingAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}