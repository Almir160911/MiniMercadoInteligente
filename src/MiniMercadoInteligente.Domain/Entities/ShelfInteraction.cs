namespace MiniMercadoInteligente.Domain.Entities;

public class ShelfInteraction
{
    public Guid Id { get; set; }

    public Guid? SessionId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? AreaId { get; set; }
    public string AreaCode { get; set; } = default!;

    public string? CameraId { get; set; }
    public string? SensorId { get; set; }
    public string? TrackId { get; set; }

    public string InteractionType { get; set; } = default!; 
    // PICK / RETURN / TOUCH / UNKNOWN

    public decimal WeightDeltaGrams { get; set; }
    public decimal Confidence { get; set; } = 0m;

    public string? EvidenceId { get; set; }
    public string PayloadJson { get; set; } = "{}";

    public bool Reconciled { get; set; } = false;

    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}