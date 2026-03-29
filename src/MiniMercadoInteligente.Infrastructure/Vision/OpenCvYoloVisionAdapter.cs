using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Infrastructure.Vision;

public class OpenCvYoloVisionAdapter : ICameraVisionPort
{
    public Task<IReadOnlyCollection<TrackingSnapshot>> DetectPeopleAsync(
        CameraDevice camera,
        CancellationToken ct = default)
    {
        IReadOnlyCollection<TrackingSnapshot> snapshots =
        [
            new TrackingSnapshot
            {
                Id = Guid.NewGuid(),
                CameraId = camera.CameraId,
                TrackId = $"trk-{Guid.NewGuid():N}",
                AreaId = camera.AreaId,
                AreaCode = camera.AreaCode,
                Confidence = 0.95m,
                CapturedAtUtc = DateTime.UtcNow,
                BoundingBox = new BoundingBox
                {
                    X = 120,
                    Y = 80,
                    Width = 70,
                    Height = 180
                }
            }
        ];

        return Task.FromResult(snapshots);
    }
}