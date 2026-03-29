namespace MiniMercadoInteligente.Application.Services;

public class BurstEventFraudRule : IFraudRule
{
    public string Name => "BurstEvent";

    public Task<IReadOnlyList<FraudFlag>> EvaluateAsync(FraudContext context, CancellationToken ct)
    {
        var suspiciousWindowSeconds = 10;

        var grouped = context.Events
            .Where(e => e.EventType is "WEIGHT_DECREASE" or "PRODUCT_SCANNED" or "QRCODE_SCANNED")
            .OrderBy(e => e.OccurredAt)
            .ToList();

        if (grouped.Count < 5)
            return Task.FromResult<IReadOnlyList<FraudFlag>>(Array.Empty<FraudFlag>());

        var first = grouped.First().OccurredAt;
        var last = grouped.Last().OccurredAt;
        var seconds = (last - first).TotalSeconds;

        if (seconds > suspiciousWindowSeconds)
            return Task.FromResult<IReadOnlyList<FraudFlag>>(Array.Empty<FraudFlag>());

        IReadOnlyList<FraudFlag> flags =
        [
            new FraudFlag(
                "BURST_ACTIVITY",
                "Muitas interações foram registradas em uma janela muito curta.",
                20,
                "MEDIUM",
                new { totalEvents = grouped.Count, seconds })
        ];

        return Task.FromResult(flags);
    }
}