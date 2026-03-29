using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface ICameraRepository
{
    Task<CameraDevice?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<CameraDevice?> GetByCameraIdAsync(string cameraId, CancellationToken ct = default);

    Task<List<CameraDevice>> GetActiveAsync(CancellationToken ct = default);

    Task<List<CameraDevice>> ListByAreaAsync(string areaCode, CancellationToken ct = default);

    Task<CameraDevice> AddAsync(CameraDevice camera, CancellationToken ct = default);

    Task<CameraDevice> UpdateAsync(CameraDevice camera, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}