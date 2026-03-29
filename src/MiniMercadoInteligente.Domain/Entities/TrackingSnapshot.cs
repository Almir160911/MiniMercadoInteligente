using MiniMercadoInteligente.Domain.ValueObjects;

namespace MiniMercadoInteligente.Domain.Entities;

public class TrackingSnapshot
{
    public Guid Id { get; set; }

    public Guid? SessionId { get; set; }

    public string CameraId { get; set; } = default!;
    public string TrackId { get; set; } = default!;

    public Guid? AreaId { get; set; }
    public string AreaCode { get; set; } = default!;

    public BoundingBox BoundingBox { get; set; } = new();

    public decimal Confidence { get; set; } = 0m;

    public DateTime CapturedAtUtc { get; set; } = DateTime.UtcNow;
}