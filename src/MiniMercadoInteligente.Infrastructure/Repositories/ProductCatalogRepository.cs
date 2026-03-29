using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class ProductCatalogRepository : IProductCatalogRepository
{
    private readonly AppDbContext _context;

    public ProductCatalogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> FindBySkuOrBarcodeAsync(string skuOrBarcode, CancellationToken ct)
{
        return await _context.Products
            .FirstOrDefaultAsync(x =>
                x.Active &&
                (x.Sku == skuOrBarcode || x.Barcode == skuOrBarcode),
                ct);
}

    public async Task<List<Product>> ListProductsAsync(CancellationToken ct)
{
    return await _context.Products
        .Where(x => x.Active)
        .OrderBy(x => x.Name)
        .ToListAsync(ct);
}
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Products.FindAsync([id], ct);

    public async Task<Product> CreateAsync(Product product, CancellationToken ct)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);
        return product;
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken ct)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(ct);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid productId, CancellationToken ct)
{
    var entity = await _context.Products
        .FirstOrDefaultAsync(x => x.ProductId == productId, ct);

    if (entity is null)
        return false;

    entity.Active = false;

    await _context.SaveChangesAsync(ct);
    return true;
}

    public Task<Product> UpsertProductAsync(Product product, CancellationToken ct)
        => CreateAsync(product, ct);

    public Task<ProductPrice> SetPriceAsync(Guid productId, ProductPrice price, CancellationToken ct)
        => Task.FromResult(price);

    public Task<decimal> GetAverageActivePriceAsync(CancellationToken ct)
        => Task.FromResult(10m);

    public Task<Product?> FindClosestByWeightDeltaAsync(string areaCode, decimal weightDeltaGrams, CancellationToken ct = default)
        => Task.FromResult<Product?>(null);
    
    public Task<Product?> FindByQrCodeAsync(string qrCode, CancellationToken ct)
    {
        return _context.Products
            .FirstOrDefaultAsync(x => x.QrCode == qrCode, ct);
    }

    public Task<Product?> FindByVisionLabelAsync(string label, CancellationToken ct)
    {
        return _context.Products
            .FirstOrDefaultAsync(x => x.Name == label, ct);
    }

    public Task<List<Product>> ListByAreaAsync(string areaCode, CancellationToken ct)
    {
        return _context.Products.ToListAsync(ct);
    }

    public Task<List<Product>> ListWeightControlledByAreaAsync(string areaCode, CancellationToken ct)
    {
        return _context.Products
            .Where(x => x.IsWeightControlled)
            .ToListAsync(ct);
    }

    public Task<List<Product>> FindCandidatesByWeightDeltaAsync(string areaCode, decimal delta, CancellationToken ct)
    {
        return _context.Products
            .Where(x => x.IsWeightControlled)
            .ToListAsync(ct);
    }

    public async Task<List<Product>> ListProductsByStatusAsync(bool? active, CancellationToken ct)
{
    var query = _context.Products.AsQueryable();

    if (active.HasValue)
        query = query.Where(x => x.Active == active.Value);

    return await query
        .OrderBy(x => x.Name)
        .ToListAsync(ct);
}
}