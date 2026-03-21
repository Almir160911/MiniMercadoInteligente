using System.Text.Json;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

public interface IFraudSnapshotService
{
    Task<FraudSessionSnapshot> BuildAsync(
        Guid sessionId,
        bool hasReplayAttempt = false,
        bool hasWebhookMismatch = false,
        CancellationToken ct = default);
}

public class FraudSnapshotService : IFraudSnapshotService
{
    private readonly ISessionRepository _sessions;
    private readonly ICartRepository _cart;
    private readonly IPaymentRepository _payments;
    private readonly IEventStore _events;

    public FraudSnapshotService(
        ISessionRepository sessions,
        ICartRepository cart,
        IPaymentRepository payments,
        IEventStore events)
    {
        _sessions = sessions;
        _cart = cart;
        _payments = payments;
        _events = events;
    }

    public async Task<FraudSessionSnapshot> BuildAsync(
        Guid sessionId,
        bool hasReplayAttempt = false,
        bool hasWebhookMismatch = false,
        CancellationToken ct = default)
    {
        var session = await _sessions.GetAsync(sessionId, ct)
            ?? throw new KeyNotFoundException("Sessão não encontrada.");

        var cartItems = await _cart.ListBySessionAsync(sessionId, ct);
        var payment = await _payments.GetLastBySessionAsync(sessionId, ct);
        var events = await _events.GetBySessionAsync(sessionId, ct);

        var paidQty = cartItems.Sum(x => x.Qty);
        var physicalEstimatedQty = 0;
        var hasWeightMismatch = false;
        var hasAreaMismatch = false;

        foreach (var ev in events)
        {
            if (ev.EventType == "PRODUCT_IDENTIFIED_BY_CAMERA")
            {
                physicalEstimatedQty += ReadInt(ev.PayloadJson, "qty", 1);
            }

            if (ev.EventType == "PRODUCT_ESTIMATED_FROM_WEIGHT")
            {
                physicalEstimatedQty += Math.Max(1, ReadInt(ev.PayloadJson, "qty", 1));

                if (ReadBool(ev.PayloadJson, "weightMismatch"))
                    hasWeightMismatch = true;
            }

            if (ev.EventType == "PRODUCT_RETURNED_WRONG_AREA" ||
                ev.EventType == "PLANOGRAM_MISMATCH")
            {
                hasAreaMismatch = true;
            }
        }

        var divergenceQty = Math.Max(0, physicalEstimatedQty - paidQty);

        var sessionAbandonedPattern =
            session.Status == SessionStatus.Open &&
            physicalEstimatedQty > 0 &&
            payment is null &&
            session.StartedAt < DateTime.UtcNow.AddMinutes(-10);

        return new FraudSessionSnapshot(
            SessionId: session.SessionId,
            ResidentId: session.ResidentId,
            DeviceId: session.DeviceId,
            PaidQty: paidQty,
            PhysicalEstimatedQty: physicalEstimatedQty,
            DistinctCartItems: cartItems.Count,
            DivergenceQty: divergenceQty,
            TotalPaidAmount: payment?.Amount ?? 0m,
            HasPayment: payment is not null,
            HasWeightMismatch: hasWeightMismatch,
            HasAreaMismatch: hasAreaMismatch,
            HasReplayAttempt: hasReplayAttempt,
            HasWebhookMismatch: hasWebhookMismatch,
            SessionAbandonedPattern: sessionAbandonedPattern
        );
    }

    private static int ReadInt(string json, string property, int fallback)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(property, out var el) && el.ValueKind == JsonValueKind.Number)
                return el.GetInt32();
        }
        catch { }
        return fallback;
    }

    private static bool ReadBool(string json, string property)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(property, out var el) && el.ValueKind == JsonValueKind.True)
                return true;
        }
        catch { }
        return false;
    }
}