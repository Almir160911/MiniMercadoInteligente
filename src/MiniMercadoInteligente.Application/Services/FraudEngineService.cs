using System.Text.Json;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

public class FraudEngineService : IFraudEngineService
{
    private readonly ISessionRepository _sessions;
    private readonly ICartRepository _cart;
    private readonly IEventStore _events;
    private readonly IPaymentRepository _payments;
    private readonly ITrackingRepository _tracking;
    private readonly IEnumerable<IFraudRule> _rules;
   
public FraudEngineService(
    ISessionRepository sessions,
    ICartRepository cart,
    IEventStore events,
    IPaymentRepository payments,
    ITrackingRepository tracking,
    IEnumerable<IFraudRule> rules)
{
    _sessions = sessions;
    _cart = cart;
    _events = events;
    _payments = payments;
    _tracking = tracking;
    _rules = rules;
}

    public async Task<FraudEvaluationResult> EvaluateAsync(Guid sessionId, CancellationToken ct)
    {
        var session = await _sessions.GetAsync(sessionId, ct)
            ?? throw new InvalidOperationException("Sessão não encontrada.");

        var cartItems = await _cart.ListBySessionAsync(sessionId, ct);
        var eventList = await _events.GetBySessionAsync(sessionId, ct);
        var lastPayment = await _payments.GetLastBySessionAsync(sessionId, ct);
        var trackingSnapshots = await _tracking.ListBySessionAsync(sessionId, ct);

        var context = new FraudContext(
            session,
            cartItems,
            eventList,
            lastPayment,
            trackingSnapshots);

        var allFlags = new List<FraudFlag>();

        foreach (var rule in _rules)
        {
            var flags = await rule.EvaluateAsync(context, ct);
            allFlags.AddRange(flags);
        }

        var fraudScore = Math.Min(100, allFlags.Sum(x => x.Score));
        var suspected = fraudScore > 0;

        return new FraudEvaluationResult(
            sessionId,
            suspected,
            fraudScore,
            allFlags);
    }

    public async Task ApplyToSessionAsync(Guid sessionId, CancellationToken ct)
    {
        var result = await EvaluateAsync(sessionId, ct);

        var session = await _sessions.GetAsync(sessionId, ct)
            ?? throw new InvalidOperationException("Sessão não encontrada.");

        session.FraudSuspected = result.FraudSuspected;
        session.FraudScore = result.FraudScore;
        session.FraudFlagsJson = JsonSerializer.Serialize(result.Flags);

        await _sessions.UpdateAsync(session, ct);
    }
}