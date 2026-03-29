namespace MiniMercadoInteligente.Application.Services;

public interface ITrackingService
{
    Task RegisterTrackingAsync(
        string cameraId,
        string trackId,
        string areaCode,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        decimal confidence,
        CancellationToken ct = default);
}