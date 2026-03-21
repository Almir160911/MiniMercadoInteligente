using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IProductCatalogRepository
{
    Task<Product?> FindBySkuOrBarcodeAsync(string skuOrBarcode, CancellationToken ct);
    Task<List<Product>> ListProductsAsync(CancellationToken ct);
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct);
    Task<Product> CreateAsync(Product product, CancellationToken ct);
    Task<Product> UpdateAsync(Product product, CancellationToken ct);
    Task<bool> DeleteAsync(Guid productId, CancellationToken ct);
    Task<Product> UpsertProductAsync(Product product, CancellationToken ct);
    Task<ProductPrice> SetPriceAsync(Guid productId, ProductPrice price, CancellationToken ct);
    Task<decimal> GetAverageActivePriceAsync(CancellationToken ct);
}
