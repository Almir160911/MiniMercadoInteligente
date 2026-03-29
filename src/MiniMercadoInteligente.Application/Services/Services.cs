using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

#region Interfaces

public interface ISessionService
{
    Task<CreateSessionResponse> CreateAsync(CreateSessionRequest req, CancellationToken ct);
    Task<SessionDetailResponse?> GetDetailAsync(Guid sessionId, CancellationToken ct);
    Task<CloseSessionResponse> CloseAsync(Guid sessionId, CloseSessionRequest req, CancellationToken ct);
}

public interface ICartService
{
    Task<CartResponse> GetAsync(Guid sessionId, CancellationToken ct);
    Task<CartItemDto> AddAsync(Guid sessionId, AddCartItemRequest req, CancellationToken ct);
    Task RemoveAsync(Guid sessionId, Guid itemId, CancellationToken ct);
}

public interface IPaymentService
{
    Task<PaymentDto> CreateAsync(Guid sessionId, CreatePaymentRequest req, CancellationToken ct);
    Task HandleWebhookAsync(PaymentWebhookRequest req, CancellationToken ct);
}

public interface IAlertService
{
    Task<List<AlertDto>> ListAsync(Guid sessionId, CancellationToken ct);
    Task<bool> ResolveAsync(Guid alertId, ResolveAlertRequest req, CancellationToken ct);
}

public interface IAdminService
{
    Task<DeviceApiKey> UpsertDeviceApiKeyAsync(UpsertDeviceApiKeyRequest req, CancellationToken ct);
}

public interface IProductCrudService
{
    Task<List<ProductResponse>> ListAsync(bool? active, CancellationToken ct);
    Task<ProductResponse?> GetAsync(Guid productId, CancellationToken ct);
    Task<ProductResponse> CreateAsync(CreateProductRequest req, CancellationToken ct);
    Task<ProductResponse> UpdateAsync(Guid productId, UpdateProductRequest req, CancellationToken ct);
    Task<bool> DeleteAsync(Guid productId, CancellationToken ct);
}

public interface IEventIngestService
{
    Task IngestAsync(IngestEventRequest req, CancellationToken ct);
}

public interface IReconciliationService
{
    Task<ReconciliationResult> RunAsync(Guid sessionId, CancellationToken ct);
}

#endregion

#region Models

public record ReconciliationResult(
    Guid SessionId,
    int EstimatedQty,
    int PaidQty,
    int DivergenceQty,
    decimal EstimatedLoss,
    double Confidence
);

#endregion

#region SessionService

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessions;
    private readonly ICartRepository _cart;
    private readonly IPaymentRepository _payments;
    private readonly IAlertRepository _alerts;
    private readonly IReconciliationService _reconciliation;

    private readonly IFraudEngineService _fraudEngine;

    public SessionService(
        ISessionRepository sessions,
        ICartRepository cart,
        IPaymentRepository payments,
        IAlertRepository alerts,
        IReconciliationService reconciliation,
        IFraudEngineService fraudEngine)
    {
        _sessions = sessions;
        _cart = cart;
        _payments = payments;
        _alerts = alerts;
        _reconciliation = reconciliation;
        _fraudEngine = fraudEngine;
    }

    public async Task<CreateSessionResponse> CreateAsync(CreateSessionRequest req, CancellationToken ct)
    {
        var openByResident = await _sessions.GetOpenByResidentAsync(req.ResidentId, ct);
        if (openByResident is not null)
            throw new InvalidOperationException("O morador já possui uma sessão aberta.");

        var openByDevice = await _sessions.GetOpenByDeviceAsync(req.DeviceId, ct);
        if (openByDevice is not null)
            throw new InvalidOperationException("O dispositivo já possui uma sessão aberta.");

        var session = new Session
        {
            SessionId = Guid.NewGuid(),
            ResidentId = req.ResidentId,
            CondominiumId = req.CondominiumId,
            DeviceId = req.DeviceId,
            Status = SessionStatus.Open,
            StartedAt = DateTime.UtcNow
        };

        await _sessions.AddAsync(session, ct);

        return new CreateSessionResponse(session.SessionId, session.Status.ToString(), session.StartedAt);
    }

    public async Task<SessionDetailResponse?> GetDetailAsync(Guid sessionId, CancellationToken ct)
    {
        var session = await _sessions.GetAsync(sessionId, ct);
        if (session is null) return null;

        var cartItems = await _cart.ListBySessionAsync(sessionId, ct);
        var payment = await _payments.GetLastBySessionAsync(sessionId, ct);
        var alerts = await _alerts.ListBySessionAsync(sessionId, ct);

        var paidQty = payment?.Status.ToString() == "Paid" ? cartItems.Sum(x => x.Qty) : 0;
        var divergenceQty = session.FraudSuspected ? session.FraudScore / 10 : 0;

        return new SessionDetailResponse(
            session.SessionId,
            session.Status.ToString(),
            session.StartedAt,
            session.EndedAt,
            paidQty,
            divergenceQty,
            cartItems.Select(MapCartItem).ToList(),
            payment is null ? null : MapPayment(payment),
            alerts.Select(MapAlert).ToList(),
            session.FraudSuspected,
            session.FraudScore
        );
    }

    public async Task<CloseSessionResponse> CloseAsync(Guid sessionId, CloseSessionRequest req, CancellationToken ct)
    {
        var session = await _sessions.GetAsync(sessionId, ct)
            ?? throw new InvalidOperationException("Sessão não encontrada.");

        if (session.Status.ToString() != "Open")
            throw new InvalidOperationException("Somente sessões abertas podem ser encerradas.");

        var reconciliation = await _reconciliation.RunAsync(sessionId, ct);

    session.Status = SessionStatus.Closed;
    session.EndedAt = DateTime.UtcNow;

    await _sessions.UpdateAsync(session, ct);
    await _fraudEngine.ApplyToSessionAsync(sessionId, ct);

    session = await _sessions.GetAsync(sessionId, ct)
        ?? throw new InvalidOperationException("Sessão não encontrada após antifraude.");

    var fraudResult = await _fraudEngine.EvaluateAsync(sessionId, ct);

    var decision = FraudScorePolicy.Decide(fraudResult.FraudScore);

    foreach (var flag in fraudResult.Flags.Where(x => x.Severity is "MEDIUM" or "HIGH"))
    {
        var alert = new Alert
        {
            AlertId = Guid.NewGuid(),
            SessionId = sessionId,
            Type = flag.Code,
            EstimatedLoss = reconciliation.EstimatedLoss,
            CreatedAt = DateTime.UtcNow,
            PayloadJson = JsonSerializer.Serialize(flag),
            Status = AlertStatus.Open,
            Severity = flag.Severity == "HIGH" ? AlertSeverity.High : AlertSeverity.Medium
        };

        await _alerts.AddAsync(alert, ct);
    }

    if (decision == FraudDecision.Block)
    {
        session = await _sessions.GetAsync(sessionId, ct)
            ?? throw new InvalidOperationException("Sessão não encontrada.");

        session.Status = SessionStatus.Blocked;
        await _sessions.UpdateAsync(session, ct);
    }
       

            return new CloseSessionResponse(
                session.SessionId,
                session.Status.ToString(),
                reconciliation.PaidQty,
                reconciliation.EstimatedQty,
                reconciliation.DivergenceQty,
                reconciliation.EstimatedLoss,
                session.EndedAt,
                session.FraudSuspected,
                session.FraudScore
            );
        }

    private static CartItemDto MapCartItem(CartItem item) =>
        new(item.CartItemId, item.ProductId, item.Sku, item.Qty, item.OccurredAtUtc, item.Source);

    private static PaymentDto MapPayment(Payment payment) =>
        new(payment.PaymentId, payment.Method, payment.Amount, payment.Status.ToString(), payment.PaidAt, payment.GatewayRef);

    private static AlertDto MapAlert(Alert alert) =>
        new(alert.AlertId, alert.Severity.ToString(), alert.Type, alert.Status.ToString(), alert.EstimatedLoss, alert.CreatedAt);
}

#endregion

#region CartService

public class CartService : ICartService
{
    private readonly ICartRepository _cart;
    private readonly IProductCatalogRepository _catalog;

    public CartService(ICartRepository cart, IProductCatalogRepository catalog)
    {
        _cart = cart;
        _catalog = catalog;
    }

    public async Task<CartResponse> GetAsync(Guid sessionId, CancellationToken ct)
    {
        var items = await _cart.ListBySessionAsync(sessionId, ct);

        return new CartResponse(
            sessionId,
            items.Select(x => new CartItemDto(
                x.CartItemId,
                x.ProductId,
                x.Sku,
                x.Qty,
                x.OccurredAtUtc,
                x.Source)).ToList()
        );
    }

    public async Task<CartItemDto> AddAsync(Guid sessionId, AddCartItemRequest req, CancellationToken ct)
{
    var product = await _catalog.FindBySkuOrBarcodeAsync(req.Sku, ct);
    if (product is null)
        throw new InvalidOperationException("Produto não encontrado para o SKU/barcode informado.");

    var item = new CartItem
    {
        CartItemId = Guid.NewGuid(),
        SessionId = sessionId,
        ProductId = product.ProductId,
        Sku = product.Sku,
        Qty = req.Qty,
        OccurredAtUtc = DateTime.UtcNow,
        Source = string.IsNullOrWhiteSpace(req.Source) ? "Totem" : req.Source
    };

    await _cart.AddAsync(item, ct);

    return new CartItemDto(
        item.CartItemId,
        item.ProductId,
        item.Sku,
        item.Qty,
        item.OccurredAtUtc,
        item.Source
    );
}

    public async Task RemoveAsync(Guid sessionId, Guid itemId, CancellationToken ct)
    {
        await _cart.RemoveAsync(sessionId, itemId, ct);
    }
}

#endregion

#region PaymentService

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _payments;
    private readonly ISessionRepository _sessions;

    public PaymentService(IPaymentRepository payments, ISessionRepository sessions)
    {
        _payments = payments;
        _sessions = sessions;
    }

    public async Task<PaymentDto> CreateAsync(Guid sessionId, CreatePaymentRequest req, CancellationToken ct)
    {
        var session = await _sessions.GetAsync(sessionId, ct)
            ?? throw new InvalidOperationException("Sessão não encontrada.");

        var payment = new Payment
        {
            PaymentId = Guid.NewGuid(),
            SessionId = session.SessionId,
            Method = req.Method,
            Amount = req.Amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _payments.AddAsync(payment, ct);

        return new PaymentDto(
            payment.PaymentId,
            payment.Method,
            payment.Amount,
            payment.Status.ToString(),
            payment.PaidAt,
            payment.GatewayRef
        );
    }

    public async Task HandleWebhookAsync(PaymentWebhookRequest req, CancellationToken ct)
    {
        var payment = await _payments.FindByWebhookAsync(req.PaymentId, req.GatewayRef, ct);
        if (payment is null) return;

        payment.Status = req.Status.Equals("paid", StringComparison.OrdinalIgnoreCase)
            ? PaymentStatus.Paid
            : PaymentStatus.Failed;

        payment.PaidAt = payment.Status == PaymentStatus.Paid ? DateTime.UtcNow : null;
        payment.GatewayRef ??= req.GatewayRef;

        await _payments.UpdateAsync(payment, ct);
    }
}

#endregion

#region AlertService

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alerts;

    public AlertService(IAlertRepository alerts)
    {
        _alerts = alerts;
    }

    public async Task<List<AlertDto>> ListAsync(Guid sessionId, CancellationToken ct)
    {
        var alerts = await _alerts.ListBySessionAsync(sessionId, ct);

        return alerts.Select(a => new AlertDto(
            a.AlertId,
            a.Severity.ToString(),
            a.Type,
            a.Status.ToString(),
            a.EstimatedLoss,
            a.CreatedAt)).ToList();
    }

    public async Task<bool> ResolveAsync(Guid alertId, ResolveAlertRequest req, CancellationToken ct)
    {
        var alert = await _alerts.GetAsync(alertId, ct);
        if (alert is null) return false;

        alert.Status = AlertStatus.Resolved;
        alert.PayloadJson = string.IsNullOrWhiteSpace(alert.PayloadJson)
            ? JsonSerializer.Serialize(new { resolution = req.Resolution })
            : alert.PayloadJson;

        await _alerts.UpdateAsync(alert, ct);
        return true;
    }
}

#endregion

#region Admin

public class AdminService : IAdminService
{
    private readonly IDeviceApiKeyRepository _repo;

    public AdminService(IDeviceApiKeyRepository repo)
    {
        _repo = repo;
    }

    public async Task<DeviceApiKey> UpsertDeviceApiKeyAsync(UpsertDeviceApiKeyRequest req, CancellationToken ct)
    {
        var entity = new DeviceApiKey
        {
            DeviceApiKeyId = Guid.NewGuid(),
            DeviceId = req.DeviceId,
            DeviceType = req.DeviceType,
            Active = req.Active,
            ApiKeyHash = Sha256(req.ApiKeyPlain)
        };

        return await _repo.UpsertAsync(entity, ct);
    }

    private static string Sha256(string value)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}

#endregion

#region Product

public class ProductCrudService : IProductCrudService
{
    private readonly IProductCatalogRepository _catalog;

    public ProductCrudService(IProductCatalogRepository catalog)
    {
        _catalog = catalog;
    }

    public async Task<List<ProductResponse>> ListAsync(bool? active, CancellationToken ct)
{
    var list = await _catalog.ListProductsByStatusAsync(active, ct);
    return list.Select(MapProduct).ToList();
}
    public async Task<ProductResponse?> GetAsync(Guid productId, CancellationToken ct)
    {
        var product = await _catalog.GetByIdAsync(productId, ct);
        return product is null ? null : MapProduct(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest req, CancellationToken ct)
    {
        var exists = await _catalog.FindBySkuOrBarcodeAsync(req.Sku, ct);
        if (exists is not null)
            throw new InvalidOperationException("Já existe produto com o mesmo SKU.");

        var entity = new Product
        {
            ProductId = Guid.NewGuid(),
            Sku = req.Sku,
            Name = req.Name,
            Barcode = req.Barcode,
            QrCode = req.QrCode,
            NominalWeightGrams = req.NominalWeightGrams,
            WeightToleranceGrams = req.WeightToleranceGrams,
            IsWeightControlled = req.IsWeightControlled,
            Active = req.Active
        };

        await _catalog.CreateAsync(entity, ct);
        return MapProduct(entity);
    }

    public async Task<ProductResponse> UpdateAsync(Guid productId, UpdateProductRequest req, CancellationToken ct)
    {
        var entity = await _catalog.GetByIdAsync(productId, ct)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        entity.Name = req.Name;
        entity.Barcode = req.Barcode;
        entity.QrCode = req.QrCode;
        entity.NominalWeightGrams = req.NominalWeightGrams;
        entity.WeightToleranceGrams = req.WeightToleranceGrams;
        entity.IsWeightControlled = req.IsWeightControlled;
        entity.Active = req.Active;

        await _catalog.UpdateAsync(entity, ct);
        return MapProduct(entity);
    }

    public async Task<bool> DeleteAsync(Guid productId, CancellationToken ct)
    {
        return await _catalog.DeleteAsync(productId, ct);
    }

    private static ProductResponse MapProduct(Product p) =>
        new(
            p.ProductId,
            p.Sku,
            p.Name,
            p.Barcode,
            p.QrCode,
            p.NominalWeightGrams,
            p.WeightToleranceGrams,
            p.IsWeightControlled,
            p.Active
        );
}

#endregion

#region EventIngestService / ReconciliationService

public class EventIngestService : IEventIngestService
{
    private readonly IEventStore _eventStore;
    private readonly IProductCatalogRepository _products;
    private readonly ICartService _cartService;

    public EventIngestService(
        IEventStore eventStore,
        IProductCatalogRepository products,
        ICartService cartService)
    {
        _eventStore = eventStore;
        _products = products;
        _cartService = cartService;
    }

    public async Task IngestAsync(IngestEventRequest req, CancellationToken ct)
    {
        var entity = new EventRecord
        {
            EventId = Guid.NewGuid(),
            EventType = req.EventType,
            SessionId = req.SessionId,
            AreaId = req.AreaId,
            ProductId = req.ProductId,
            OccurredAt = req.OccurredAt ?? DateTime.UtcNow,
            Source = req.Source,
            PayloadJson = string.IsNullOrWhiteSpace(req.PayloadJson) ? "{}" : req.PayloadJson
        };

        await _eventStore.AppendAsync(entity, ct);

        if (!req.SessionId.HasValue)
            return;

        if (!ShouldProjectToCart(req.EventType))
            return;

        var payload = ParsePayload(req.PayloadJson);

        var sku = TryReadString(payload, "sku");
        var qty = TryReadInt(payload, "qty") ?? 1;

        if (string.IsNullOrWhiteSpace(sku) && req.ProductId.HasValue)
        {
            var product = await _products.GetByIdAsync(req.ProductId.Value, ct);
            sku = product?.Sku;
        }

        if (string.IsNullOrWhiteSpace(sku))
            return;

        await _cartService.AddAsync(
            req.SessionId.Value,
            new AddCartItemRequest(sku, qty, req.Source),
            ct);
    }

    private static bool ShouldProjectToCart(string eventType)
    {
        return eventType is "QRCODE_SCANNED" or "PRODUCT_SCANNED" or "WEIGHT_DECREASE";
    }

    private static JsonElement? ParsePayload(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch
        {
            return null;
        }
    }

    private static string? TryReadString(JsonElement? root, string property)
    {
        if (root is null) return null;
        if (!root.Value.TryGetProperty(property, out var value)) return null;
        return value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    }

    private static int? TryReadInt(JsonElement? root, string property)
    {
        if (root is null) return null;
        if (!root.Value.TryGetProperty(property, out var value)) return null;
        return value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var n) ? n : null;
    }
}

public class ReconciliationService : IReconciliationService
{
    private readonly IEventStore _eventStore;
    private readonly ICartRepository _cartRepository;

    public ReconciliationService(
        IEventStore eventStore,
        ICartRepository cartRepository)
    {
        _eventStore = eventStore;
        _cartRepository = cartRepository;
    }

    public async Task<ReconciliationResult> RunAsync(Guid sessionId, CancellationToken ct)
    {
        var events = await _eventStore.GetBySessionAsync(sessionId, ct);
        var cartItems = await _cartRepository.ListBySessionAsync(sessionId, ct);

        var estimatedQty = events.Count(e =>
            e.EventType == "PRODUCT_SCANNED" ||
            e.EventType == "QRCODE_SCANNED" ||
            e.EventType == "WEIGHT_DECREASE");

        var paidQty = cartItems.Sum(x => x.Qty);
        var divergenceQty = Math.Max(0, estimatedQty - paidQty);
        var estimatedLoss = divergenceQty * 10m;

        var confidence = estimatedQty == 0
            ? 1d
            : Math.Max(0d, 1d - ((double)divergenceQty / estimatedQty));

        return new ReconciliationResult(
            sessionId,
            estimatedQty,
            paidQty,
            divergenceQty,
            estimatedLoss,
            confidence
        );
    }
}

#endregion