using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IDeviceApiKeyRepository
{
    Task<DeviceApiKey?> GetAsync(Guid deviceApiKeyId, CancellationToken ct = default);
    Task<DeviceApiKey?> GetByDeviceIdAsync(string deviceId, CancellationToken ct = default);
    Task<DeviceApiKey?> FindByHashAsync(string apiKeyHash, CancellationToken ct = default);
    Task<DeviceApiKey> UpsertAsync(DeviceApiKey entity, CancellationToken ct);
}