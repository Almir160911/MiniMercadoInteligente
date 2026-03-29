namespace MiniMercadoInteligente.Application.Services;

public class WeightEventWithoutCartFraudRule : IFraudRule
{
    public string Name => "WeightEventWithoutCart";

    public Task<IReadOnlyList<FraudFlag>> EvaluateAsync(FraudContext context, CancellationToken ct)
    {
        var weightEvents = context.Events.Count(e => e.EventType == "WEIGHT_DECREASE");
        var cartQty = context.CartItems.Sum(x => x.Qty);

        if (weightEvents == 0 || cartQty >= weightEvents)
            return Task.FromResult<IReadOnlyList<FraudFlag>>(Array.Empty<FraudFlag>());

        var missing = weightEvents - cartQty;

        IReadOnlyList<FraudFlag> flags =
        [
            new FraudFlag(
                "WEIGHT_WITHOUT_CART",
                $"Eventos de peso sem correspondência suficiente no carrinho. Faltantes={missing}.",
                missing >= 2 ? 30 : 15,
                missing >= 2 ? "MEDIUM" : "LOW",
                new { weightEvents, cartQty, missing })
        ];

        return Task.FromResult(flags);
    }
}