using System.Text.Json;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

public record FraudCheckResult(
    bool FraudSuspected,
    int FraudScore,
    bool RequireManualReview,
    bool BlockSessionClose,
    bool SuspectedTheft,
    List<string> Reasons)
{
    public int RiskScore => FraudScore;

    public string RiskLevel =>
        FraudScore >= 80 ? "Critical" :
        FraudScore >= 60 ? "High" :
        FraudScore >= 30 ? "Medium" :
        "Low";
}

public interface IFraudService
{
    Task<FraudCheckResult> AnalyzeSessionAsync(Guid sessionId, CancellationToken ct);
    Task<FraudCheckResult> AnalyzeBeforeCloseAsync(Guid sessionId, int paidQty, int estimatedQty, CancellationToken ct);
}

public class FraudService : IFraudService
{
    private readonly ISessionRepository _sessions;
    private readonly ICartRepository _cart;
    private readonly IPaymentRepository _payments;
    private readonly IEventStore _events;

    public FraudService(
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

    public async Task<FraudCheckResult> AnalyzeSessionAsync(Guid sessionId, CancellationToken ct)
    {
        var session = await _sessions.GetAsync(sessionId, ct);
        if (session is null)
        {
            return new FraudCheckResult(
                FraudSuspected: true,
                FraudScore: 90,
                RequireManualReview: true,
                BlockSessionClose: true,
                SuspectedTheft: true,
                Reasons: new List<string> { "Sessão inexistente." });
        }

        var cartItems = await _cart.ListBySessionAsync(sessionId, ct);
        var payment = await _payments.GetLastBySessionAsync(sessionId, ct);
        var events = await _events.GetBySessionAsync(sessionId, ct);

        var score = 0;
        var reasons = new List<string>();

        var totalQty = cartItems.Sum(x => x.Qty);

        if (totalQty <= 0)
        {
            score += 10;
            reasons.Add("Sessão sem itens no carrinho.");
        }

        if (payment is null)
        {
            score += 35;
            reasons.Add("Sessão sem pagamento registrado.");
        }
        else if (payment.Status != PaymentStatus.Paid)
        {
            score += 30;
            reasons.Add("Pagamento não confirmado como pago.");
        }

        var cameraEvents = events.Count(x => x.EventType == "PRODUCT_IDENTIFIED_BY_CAMERA");
        var weightEvents = events.Count(x => x.EventType == "PRODUCT_ESTIMATED_FROM_WEIGHT");

        if ((cameraEvents + weightEvents) > totalQty && totalQty > 0)
        {
            score += 20;
            reasons.Add("Mais eventos de detecção do que itens pagos.");
        }

        var suspiciousBurst = DetectBurstScan(cartItems);
        if (suspiciousBurst)
        {
            score += 10;
            reasons.Add("Múltiplos itens adicionados em intervalo muito curto.");
        }

        var repeatedSameSku = cartItems
            .GroupBy(x => x.Sku)
            .Any(g => g.Sum(i => i.Qty) >= 10);

        if (repeatedSameSku)
        {
            score += 10;
            reasons.Add("Quantidade elevada do mesmo SKU na mesma sessão.");
        }

        var inconsistentSources = cartItems
            .Select(x => x.Source)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() > 2;

        if (inconsistentSources)
        {
            score += 5;
            reasons.Add("Múltiplas origens de leitura na mesma sessão.");
        }

        var suspiciousWeightMismatch = DetectSuspiciousWeightPatterns(events);
        if (suspiciousWeightMismatch)
        {
            score += 15;
            reasons.Add("Padrões inconsistentes em eventos de peso.");
        }

        var suspectedTheft = score >= 60;
        var requireManualReview = score >= 40;
        var blockSessionClose = score >= 80;

        return new FraudCheckResult(
            FraudSuspected: score >= 30,
            FraudScore: Math.Min(score, 100),
            RequireManualReview: requireManualReview,
            BlockSessionClose: blockSessionClose,
            SuspectedTheft: suspectedTheft,
            Reasons: reasons);
    }

    public async Task<FraudCheckResult> AnalyzeBeforeCloseAsync(
        Guid sessionId,
        int paidQty,
        int estimatedQty,
        CancellationToken ct)
    {
        var baseResult = await AnalyzeSessionAsync(sessionId, ct);

        var score = baseResult.FraudScore;
        var reasons = new List<string>(baseResult.Reasons);

        var divergence = Math.Max(0, estimatedQty - paidQty);

        if (divergence >= 1)
        {
            score += 20;
            reasons.Add($"Divergência entre itens estimados e pagos: {divergence}.");
        }

        if (divergence >= 3)
        {
            score += 20;
            reasons.Add("Divergência alta na finalização da sessão.");
        }

        var suspectedTheft = score >= 60;
        var requireManualReview = score >= 40;
        var blockSessionClose = score >= 80;

        return new FraudCheckResult(
            FraudSuspected: score >= 30,
            FraudScore: Math.Min(score, 100),
            RequireManualReview: requireManualReview,
            BlockSessionClose: blockSessionClose,
            SuspectedTheft: suspectedTheft,
            Reasons: reasons);
    }

    private static bool DetectBurstScan(List<CartItem> items)
    {
        if (items.Count < 3)
            return false;

        var ordered = items.OrderBy(x => x.ScannedAt).ToList();

        for (var i = 2; i < ordered.Count; i++)
        {
            var delta = ordered[i].ScannedAt - ordered[i - 2].ScannedAt;
            if (delta.TotalSeconds <= 2)
                return true;
        }

        return false;
    }

    private static bool DetectSuspiciousWeightPatterns(List<EventRecord> events)
    {
        var weightEvents = events
            .Where(x => x.EventType == "PRODUCT_ESTIMATED_FROM_WEIGHT")
            .ToList();

        foreach (var ev in weightEvents)
        {
            if (string.IsNullOrWhiteSpace(ev.PayloadJson))
                continue;

            try
            {
                using var doc = JsonDocument.Parse(ev.PayloadJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("weightDeltaGrams", out var weightEl) &&
                    weightEl.ValueKind == JsonValueKind.Number)
                {
                    var grams = weightEl.GetInt32();

                    if (grams == 0)
                        return true;

                    if (Math.Abs(grams) > 50000)
                        return true;
                }
            }
            catch
            {
                return true;
            }
        }

        return false;
    }
}