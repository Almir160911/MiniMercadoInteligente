using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class DeviceApiKeyRepository : IDeviceApiKeyRepository
{
    private readonly AppDbContext _context;

    public DeviceApiKeyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DeviceApiKey?> GetAsync(Guid deviceApiKeyId, CancellationToken ct = default)
    {
        return await _context.DeviceApiKeys
            .FirstOrDefaultAsync(x => x.DeviceApiKeyId == deviceApiKeyId, ct);
    }

    public async Task<DeviceApiKey?> GetByDeviceIdAsync(string deviceId, CancellationToken ct = default)
    {
        return await _context.DeviceApiKeys
            .FirstOrDefaultAsync(x => x.DeviceId == deviceId, ct);
    }

    public async Task<DeviceApiKey?> FindByHashAsync(string apiKeyHash, CancellationToken ct = default)
    {
        return await _context.DeviceApiKeys
            .FirstOrDefaultAsync(x => x.ApiKeyHash == apiKeyHash && x.Active, ct);
    }

    public async Task<DeviceApiKey> UpsertAsync(DeviceApiKey entity, CancellationToken ct)
    {
        var existing = await _context.DeviceApiKeys
            .FirstOrDefaultAsync(x => x.DeviceId == entity.DeviceId, ct);

        if (existing is null)
        {
            _context.DeviceApiKeys.Add(entity);
            await _context.SaveChangesAsync(ct);
            return entity;
        }

        existing.DeviceType = entity.DeviceType;
        existing.ApiKeyHash = entity.ApiKeyHash;
        existing.Active = entity.Active;

        await _context.SaveChangesAsync(ct);
        return existing;
    }
}