namespace MiniMercadoInteligente.Domain.Entities;

public class Product
{
    public Guid ProductId { get; set; }

    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;

    public string Barcode { get; set; } = default!;
    public string? QrCode { get; set; }

    // Localização física
    public string AreaCode { get; set; } = default!;

    // Peso
    public int? NominalWeightGrams { get; set; }
    public int WeightToleranceGrams { get; set; } = 50;

    public int MinWeightGrams => (NominalWeightGrams ?? 0) - WeightToleranceGrams;
    public int MaxWeightGrams => (NominalWeightGrams ?? 0) + WeightToleranceGrams;

    public bool IsWeightControlled { get; set; }

    // Preço
    public decimal CurrentPrice { get; set; }

    // Peso unitário (para cálculo automático)
    public decimal UnitWeightGrams { get; set; }

    public bool IsSoldByUnit { get; set; } = true;

    // IA / visão
    public string? VisionLabel { get; set; }
    public decimal DetectionConfidenceThreshold { get; set; } = 0.85m;

    // Risco / fraude
    public bool IsHighRisk { get; set; } = false;

    public bool Active { get; set; } = true;
}