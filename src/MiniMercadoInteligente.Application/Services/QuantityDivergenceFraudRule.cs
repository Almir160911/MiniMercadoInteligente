namespace MiniMercadoInteligente.Application.Services;

public class QuantityDivergenceFraudRule : IFraudRule
{
    public string Name => "QuantityDivergence";

    public Task<IReadOnlyList<FraudFlag>> EvaluateAsync(FraudContext context, CancellationToken ct)
    {
        var estimatedQty = context.Events.Count(e =>
            e.EventType == "PRODUCT_SCANNED" ||
            e.EventType == "QRCODE_SCANNED" ||
            e.EventType == "WEIGHT_DECREASE");

        var paidQty = context.CartItems.Sum(x => x.Qty);
        var divergence = Math.Max(0, estimatedQty - paidQty);

        if (divergence <= 0)
            return Task.FromResult<IReadOnlyList<FraudFlag>>(Array.Empty<FraudFlag>());

        var severity = divergence >= 3 ? "HIGH" : divergence == 2 ? "MEDIUM" : "LOW";
        var score = divergence >= 3 ? 60 : divergence == 2 ? 35 : 15;

        IReadOnlyList<FraudFlag> flags =
        [
            new FraudFlag(
                "QTY_DIVERGENCE",
                $"Divergência de quantidade detectada. Estimado={estimatedQty}, Pago={paidQty}.",
                score,
                severity,
                new { estimatedQty, paidQty, divergence })
        ];

        return Task.FromResult(flags);
    }
}