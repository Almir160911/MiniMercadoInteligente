namespace MiniMercadoInteligente.Application.Services;

public interface ISmartCartProjectionService
{
    Task ApplyProductPickedAsync(
        Guid sessionId,
        Guid productId,
        int qty,
        string source,
        string? sensorId,
        string? cameraId,
        string? areaCode,
        string? evidenceId,
        decimal confidence,
        CancellationToken ct = default);

    Task ApplyProductReturnedAsync(
        Guid sessionId,
        Guid productId,
        int qty,
        string source,
        string? sensorId,
        string? cameraId,
        string? areaCode,
        string? evidenceId,
        decimal confidence,
        CancellationToken ct = default);
}