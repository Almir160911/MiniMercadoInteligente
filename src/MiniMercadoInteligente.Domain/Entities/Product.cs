namespace MiniMercadoInteligente.Domain.Entities;

public class Product
{
    public Guid ProductId { get; set; }

    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;

    public string Barcode { get; set; } = default!;
    public string? QrCode { get; set; }

    public int? NominalWeightGrams { get; set; }
    public int WeightToleranceGrams { get; set; } = 50;

    public bool IsWeightControlled { get; set; }
    public bool Active { get; set; } = true;
}
