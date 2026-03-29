using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Application.Services;

public record FraudContext(
    Session Session,
    IReadOnlyList<CartItem> CartItems,
    IReadOnlyList<EventRecord> Events,
    Payment? LastPayment,
    IReadOnlyList<TrackingSnapshot> TrackingSnapshots
);

public record FraudFlag(
    string Code,
    string Message,
    int Score,
    string Severity,
    object? Metadata = null
);

public record FraudEvaluationResult(
    Guid SessionId,
    bool FraudSuspected,
    int FraudScore,
    IReadOnlyList<FraudFlag> Flags
);

public enum FraudDecision
{
    Allow,
    Alert,
    Block
}

public interface IFraudRule
{
    string Name { get; }
    Task<IReadOnlyList<FraudFlag>> EvaluateAsync(FraudContext context, CancellationToken ct);
}

public interface IFraudEngineService
{
    Task<FraudEvaluationResult> EvaluateAsync(Guid sessionId, CancellationToken ct);
    Task ApplyToSessionAsync(Guid sessionId, CancellationToken ct);
}