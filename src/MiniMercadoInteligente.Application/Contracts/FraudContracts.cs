namespace MiniMercadoInteligente.Application.Contracts;

public record FraudCheckResult(
    int RiskScore,
    string RiskLevel,
    bool BlockSessionClose,
    bool RequireManualReview,
    bool SuspectedTheft,
    List<string> Reasons
);

public record FraudSessionSnapshot(
    Guid SessionId,
    Guid? ResidentId,
    string DeviceId,
    int PaidQty,
    int PhysicalEstimatedQty,
    int DistinctCartItems,
    int DivergenceQty,
    decimal TotalPaidAmount,
    bool HasPayment,
    bool HasWeightMismatch,
    bool HasAreaMismatch,
    bool HasReplayAttempt,
    bool HasWebhookMismatch,
    bool SessionAbandonedPattern
);