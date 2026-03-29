namespace MiniMercadoInteligente.Domain.Entities;

public class CartItem
{
    public Guid CartItemId { get; set; }
    public Guid SessionId { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = default!;
    public int Qty { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public string Source { get; set; } = "UNKNOWN";
    public string? SensorId { get; set; }
    public string? CameraId { get; set; }
    public string? AreaCode { get; set; }
    public string Operation { get; set; } = "ADD";
    public string? EvidenceId { get; set; }
    public decimal Confidence { get; set; } = 1.0m;
    public bool IsReconciled { get; set; } = false;
}