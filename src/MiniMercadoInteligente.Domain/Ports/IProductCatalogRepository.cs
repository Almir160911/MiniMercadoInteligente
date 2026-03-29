using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IProductCatalogRepository
{
    Task<Product?> FindBySkuOrBarcodeAsync(string skuOrBarcode, CancellationToken ct);
    Task<Product?> FindByQrCodeAsync(string qrCode, CancellationToken ct = default);
    Task<Product?> FindByVisionLabelAsync(string visionLabel, CancellationToken ct = default);

    Task<List<Product>> ListProductsAsync(CancellationToken ct);
    Task<List<Product>> ListProductsByStatusAsync(bool? active, CancellationToken ct);

    Task<List<Product>> ListByAreaAsync(string areaCode, CancellationToken ct = default);
    Task<List<Product>> ListWeightControlledByAreaAsync(string areaCode, CancellationToken ct = default);

    Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct = default);

    Task<Product> CreateAsync(Product product, CancellationToken ct);
    Task<Product> UpdateAsync(Product product, CancellationToken ct);
    Task<bool> DeleteAsync(Guid productId, CancellationToken ct);
    Task<Product> UpsertProductAsync(Product product, CancellationToken ct);

    Task<ProductPrice> SetPriceAsync(Guid productId, ProductPrice price, CancellationToken ct);
    Task<decimal> GetAverageActivePriceAsync(CancellationToken ct);

    Task<Product?> FindClosestByWeightDeltaAsync(string areaCode, decimal weightDeltaGrams, CancellationToken ct = default);
    Task<List<Product>> FindCandidatesByWeightDeltaAsync(string areaCode, decimal weightDeltaGrams, CancellationToken ct = default);
}