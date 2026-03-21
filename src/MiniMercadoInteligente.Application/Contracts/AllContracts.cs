namespace MiniMercadoInteligente.Application.Contracts;

public record CreateSessionRequest(Guid ResidentId, Guid CondominiumId, string DeviceId);
public record CreateSessionResponse(Guid SessionId, string Status, DateTime StartedAt);

public record CloseSessionRequest(string? Reason);
public record CloseSessionResponse(
    Guid SessionId,
    string Status,
    int PaidQty,
    int EstimatedQty,
    int DivergenceQty,
    decimal EstimatedLoss,
    DateTime? EndedAt,
    bool FraudSuspected,
    int FraudScore
);

public record CartItemDto(
    Guid CartItemId,
    Guid ProductId,
    string Sku,
    int Qty,
    DateTime ScannedAt,
    string Source
);

public record CartResponse(Guid SessionId, List<CartItemDto> Items);

public record AddCartItemRequest(string Sku, int Qty, string? Source);

public record PaymentDto(
    Guid PaymentId,
    string Method,
    decimal Amount,
    string Status,
    DateTime? PaidAt,
    string? GatewayRef
);

public record CreatePaymentRequest(string Method, decimal Amount);

public record PaymentWebhookRequest(Guid? PaymentId, string? GatewayRef, string Status);

public record AlertDto(
    Guid AlertId,
    string Severity,
    string Type,
    string Status,
    decimal EstimatedLoss,
    DateTime CreatedAt
);

public record SessionDetailResponse(
    Guid SessionId,
    string Status,
    DateTime StartedAt,
    DateTime? EndedAt,
    int PaidQty,
    int DivergenceQty,
    List<CartItemDto> CartItems,
    PaymentDto? Payment,
    List<AlertDto> Alerts,
    bool FraudSuspected,
    int FraudScore
);

public record IngestEventRequest(
    string EventType,
    Guid? SessionId,
    Guid? AreaId,
    Guid? ProductId,
    DateTime? OccurredAt,
    string Source,
    string PayloadJson
);

public record ResolveAlertRequest(string Resolution);

public record UpsertDeviceApiKeyRequest(
    string DeviceId,
    string DeviceType,
    string ApiKeyPlain,
    bool Active
);

public record ProductResponse(
    Guid ProductId,
    string Sku,
    string Name,
    string? Barcode,
    string? QrCode,
    int? NominalWeightGrams,
    int WeightToleranceGrams,
    bool IsWeightControlled,
    bool Active
);

public record CreateProductRequest(
    string Sku,
    string Name,
    string? Barcode,
    string? QrCode,
    int? NominalWeightGrams,
    int WeightToleranceGrams,
    bool IsWeightControlled,
    bool Active
);

public record UpdateProductRequest(
    string Name,
    string? Barcode,
    string? QrCode,
    int? NominalWeightGrams,
    int WeightToleranceGrams,
    bool IsWeightControlled,
    bool Active
);

public record EventDto(
    Guid EventId,
    string EventType,
    Guid? SessionId,
    Guid? AreaId,
    Guid? ProductId,
    DateTime OccurredAt,
    string Source,
    string PayloadJson
);