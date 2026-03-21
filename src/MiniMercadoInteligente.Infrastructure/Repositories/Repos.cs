using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _db;

    public SessionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Session?> GetAsync(Guid sessionId, CancellationToken ct)
    {
        return await _db.Sessions.FirstOrDefaultAsync(x => x.SessionId == sessionId, ct);
    }

    public async Task<Session?> GetOpenByResidentAsync(Guid residentId, CancellationToken ct)
    {
        return await _db.Sessions
            .FirstOrDefaultAsync(x => x.ResidentId == residentId && x.Status.ToString() == "Open", ct);
    }

    public async Task<Session?> GetOpenByDeviceAsync(string deviceId, CancellationToken ct)
    {
        return await _db.Sessions
            .FirstOrDefaultAsync(x => x.DeviceId == deviceId && x.Status.ToString() == "Open", ct);
    }

    public async Task<List<Session>> ListAsync(DateTime? from, DateTime? to, string? status, CancellationToken ct)
    {
        var query = _db.Sessions.AsQueryable();

        if (from.HasValue)
            query = query.Where(x => x.StartedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.StartedAt <= to.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status.ToString() == status);

        return await query
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);
    }

    public async Task<Session> AddAsync(Session session, CancellationToken ct)
    {
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync(ct);
        return session;
    }

    public async Task<Session> UpdateAsync(Session session, CancellationToken ct)
    {
        _db.Sessions.Update(session);
        await _db.SaveChangesAsync(ct);
        return session;
    }
}

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public CartRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CartItem>> ListBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _db.CartItems
            .Where(x => x.SessionId == sessionId)
            .OrderBy(x => x.ScannedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(CartItem item, CancellationToken ct)
    {
        _db.CartItems.Add(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid sessionId, Guid itemId, CancellationToken ct)
    {
        var entity = await _db.CartItems
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.CartItemId == itemId, ct);

        if (entity is null)
            return;

        _db.CartItems.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _db;

    public PaymentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Payment?> GetLastBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _db.Payments
            .Where(x => x.SessionId == sessionId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Payment?> FindByWebhookAsync(Guid? paymentId, string? gatewayRef, CancellationToken ct)
    {
        if (paymentId.HasValue)
        {
            var byId = await _db.Payments.FirstOrDefaultAsync(x => x.PaymentId == paymentId.Value, ct);
            if (byId is not null)
                return byId;
        }

        if (!string.IsNullOrWhiteSpace(gatewayRef))
        {
            return await _db.Payments.FirstOrDefaultAsync(x => x.GatewayRef == gatewayRef, ct);
        }

        return null;
    }

    public async Task AddAsync(Payment payment, CancellationToken ct)
    {
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Payment payment, CancellationToken ct)
    {
        _db.Payments.Update(payment);
        await _db.SaveChangesAsync(ct);
    }
}

public class ProductCatalogRepository : IProductCatalogRepository
{
    private readonly AppDbContext _db;

    public ProductCatalogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Product>> ListProductsAsync(CancellationToken ct)
    {
        return await _db.Products
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct)
    {
        return await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId, ct);
    }

    public async Task<Product?> FindBySkuOrBarcodeAsync(string value, CancellationToken ct)
    {
        return await _db.Products.FirstOrDefaultAsync(x =>
            x.Sku == value ||
            x.Barcode == value ||
            x.QrCode == value, ct);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct)
    {
        return await _db.Products.FirstOrDefaultAsync(x => x.Sku == sku, ct);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken ct)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
        return product;
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken ct)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync(ct);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid productId, CancellationToken ct)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId, ct);
        if (entity is null)
            return false;

        _db.Products.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Product> UpsertProductAsync(Product product, CancellationToken ct)
    {
        var existing = await _db.Products
            .FirstOrDefaultAsync(x => x.ProductId == product.ProductId || x.Sku == product.Sku, ct);

        if (existing is null)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync(ct);
            return product;
        }

        existing.Name = product.Name;
        existing.Barcode = product.Barcode;
        existing.QrCode = product.QrCode;
        existing.NominalWeightGrams = product.NominalWeightGrams;
        existing.WeightToleranceGrams = product.WeightToleranceGrams;
        existing.IsWeightControlled = product.IsWeightControlled;
        existing.Active = product.Active;

        _db.Products.Update(existing);
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<ProductPrice> SetPriceAsync(Guid productId, ProductPrice price, CancellationToken ct)
    {
        var existing = await _db.ProductPrices
            .FirstOrDefaultAsync(x => x.ProductId == productId, ct);

        if (existing is null)
        {
            price.ProductId = productId;
            _db.ProductPrices.Add(price);
            await _db.SaveChangesAsync(ct);
            return price;
        }

        existing.Price = price.Price;
        existing.Active = price.Active;

        _db.ProductPrices.Update(existing);

await _db.SaveChangesAsync(ct);
return existing;
        _db.ProductPrices.Update(existing);
await _db.SaveChangesAsync(ct);
return existing;
        _db.ProductPrices.Update(existing);
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<decimal> GetAverageActivePriceAsync(CancellationToken ct)
    {
        var query = _db.ProductPrices.Where(x => x.Active);

        if (!await query.AnyAsync(ct))
            return 0m;

        return await query.AverageAsync(x => x.Price, ct);
    }
}

public class EventStoreRepository : IEventStore
{
    private readonly AppDbContext _db;

    public EventStoreRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AppendAsync(EventRecord record, CancellationToken ct)
    {
        _db.EventRecords.Add(record);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<EventRecord>> GetBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _db.EventRecords
            .Where(x => x.SessionId == sessionId)
            .OrderBy(x => x.OccurredAt)
            .ToListAsync(ct);
    }
}

public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _db;

    public AlertRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Alert?> GetAsync(Guid alertId, CancellationToken ct)
    {
        return await _db.Alerts.FirstOrDefaultAsync(x => x.AlertId == alertId, ct);
    }

    public async Task<List<Alert>> ListBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _db.Alerts
            .Where(x => x.SessionId == sessionId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Alert>> ListAsync(
        string? status,
        string? type,
        DateTime? from,
        DateTime? to,
        CancellationToken ct)
    {
        var query = _db.Alerts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status.ToString() == status);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(x => x.Type == type);

        if (from.HasValue)
            query = query.Where(x => x.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.CreatedAt <= to.Value);

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Alert> AddAsync(Alert alert, CancellationToken ct)
    {
        _db.Alerts.Add(alert);
        await _db.SaveChangesAsync(ct);
        return alert;
    }

    public async Task<Alert> UpdateAsync(Alert alert, CancellationToken ct)
    {
        _db.Alerts.Update(alert);
        await _db.SaveChangesAsync(ct);
        return alert;
    }
}

public class DeviceApiKeyRepository : IDeviceApiKeyRepository
{
    private readonly AppDbContext _db;

    public DeviceApiKeyRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DeviceApiKey?> FindByHashAsync(string hash, CancellationToken ct)
    {
        return await _db.DeviceApiKeys
            .FirstOrDefaultAsync(x => x.ApiKeyHash == hash, ct);
    }

    public async Task<DeviceApiKey?> FindByDeviceIdAsync(string deviceId, CancellationToken ct)
    {
        return await _db.DeviceApiKeys
            .FirstOrDefaultAsync(x => x.DeviceId == deviceId, ct);
    }

    public async Task<DeviceApiKey> UpsertAsync(DeviceApiKey entity, CancellationToken ct)
    {
        var existing = await _db.DeviceApiKeys
            .FirstOrDefaultAsync(x => x.DeviceId == entity.DeviceId, ct);

        if (existing is null)
        {
            _db.DeviceApiKeys.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        existing.DeviceType = entity.DeviceType;
        existing.Active = entity.Active;
        existing.ApiKeyHash = entity.ApiKeyHash;

        _db.DeviceApiKeys.Update(existing);
        await _db.SaveChangesAsync(ct);
        return existing;
    }
}