using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

public class SmartCartProjectionService : ISmartCartProjectionService
{
    private readonly ICartRepository _cart;

    public SmartCartProjectionService(ICartRepository cart)
    {
        _cart = cart;
    }

    public async Task ApplyProductPickedAsync(
        Guid sessionId,
        Guid productId,
        int qty,
        string source,
        string? sensorId,
        string? cameraId,
        string? areaCode,
        string? evidenceId,
        decimal confidence,
        CancellationToken ct = default)
    {
        await _cart.AddOrIncrementItemAsync(sessionId, productId, qty, 0, ct);
    }

    public async Task ApplyProductReturnedAsync(
        Guid sessionId,
        Guid productId,
        int qty,
        string source,
        string? sensorId,
        string? cameraId,
        string? areaCode,
        string? evidenceId,
        decimal confidence,
        CancellationToken ct = default)
    {
        await _cart.DecrementOrRemoveItemAsync(sessionId, productId, qty, ct);
    }
}