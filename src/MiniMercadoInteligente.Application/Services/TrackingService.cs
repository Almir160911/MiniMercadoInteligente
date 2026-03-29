using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

public class TrackingService : ITrackingService
{
    private readonly ITrackingRepository _tracking;
    private readonly ISessionResolver _resolver;

    public TrackingService(
        ITrackingRepository tracking,
        ISessionResolver resolver)
    {
        _tracking = tracking;
        _resolver = resolver;
    }

    public async Task RegisterTrackingAsync(
        string cameraId,
        string trackId,
        string areaCode,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        decimal confidence,
        CancellationToken ct = default)
    {
        var sessionId = await _resolver.ResolveSessionByTrackAsync(trackId, areaCode, ct);

        var snapshot = new TrackingSnapshot
        {
            CameraId = cameraId,
            TrackId = trackId,
            AreaCode = areaCode,
            SessionId = sessionId,
            Confidence = confidence,
            CapturedAtUtc = DateTime.UtcNow,
            BoundingBox = new BoundingBox
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            }
        };

        await _tracking.AddAsync(snapshot, ct);
    }
}