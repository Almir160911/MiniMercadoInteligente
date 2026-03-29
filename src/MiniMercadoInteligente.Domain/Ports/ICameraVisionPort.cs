using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface ICameraVisionPort
{
    Task<IReadOnlyCollection<TrackingSnapshot>> DetectPeopleAsync(
        CameraDevice camera,
        CancellationToken ct = default);
}