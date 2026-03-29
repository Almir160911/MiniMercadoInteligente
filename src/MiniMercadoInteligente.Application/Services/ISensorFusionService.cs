namespace MiniMercadoInteligente.Application.Services;

public interface ISensorFusionService
{
    Task ProcessWeightChangeAsync(
        string sensorId,
        string areaCode,
        decimal previousWeight,
        decimal currentWeight,
        string nearestTrackId,
        CancellationToken ct = default);
}