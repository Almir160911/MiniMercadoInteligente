using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IDeviceApiKeyRepository
{
    Task<DeviceApiKey?> FindByHashAsync(string apiKeyHash, CancellationToken ct);
    Task<DeviceApiKey?> FindByDeviceIdAsync(string deviceId, CancellationToken ct);
    Task<DeviceApiKey> UpsertAsync(DeviceApiKey k, CancellationToken ct);
}