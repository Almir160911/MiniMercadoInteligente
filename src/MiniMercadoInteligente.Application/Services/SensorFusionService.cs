using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

public class SensorFusionService : ISensorFusionService
{
    private readonly IProductCatalogRepository _products;
    private readonly ISessionResolver _resolver;
    private readonly ISmartCartProjectionService _cart;

    public SensorFusionService(
        IProductCatalogRepository products,
        ISessionResolver resolver,
        ISmartCartProjectionService cart)
    {
        _products = products;
        _resolver = resolver;
        _cart = cart;
    }

    public async Task ProcessWeightChangeAsync(
        string sensorId,
        string areaCode,
        decimal previousWeight,
        decimal currentWeight,
        string nearestTrackId,
        CancellationToken ct = default)
    {
        var delta = previousWeight - currentWeight;

        if (Math.Abs(delta) < 5)
            return;

        var sessionId = await _resolver.ResolveSessionByTrackAsync(nearestTrackId, areaCode, ct);

        if (sessionId is null)
            return;

        var candidates = await _products.FindCandidatesByWeightDeltaAsync(areaCode, Math.Abs(delta), ct);

        var product = candidates.FirstOrDefault();
        if (product is null)
            return;

        var qty = 1;

        if (delta > 0)
        {
            await _cart.ApplyProductPickedAsync(
                sessionId.Value,
                product.ProductId,
                qty,
                "WEIGHT",
                sensorId,
                null,
                areaCode,
                null,
                0.9m,
                ct);
        }
        else
        {
            await _cart.ApplyProductReturnedAsync(
                sessionId.Value,
                product.ProductId,
                qty,
                "WEIGHT",
                sensorId,
                null,
                areaCode,
                null,
                0.9m,
                ct);
        }
    }
}