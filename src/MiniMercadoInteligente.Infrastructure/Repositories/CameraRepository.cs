using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class CameraRepository : ICameraRepository
{
    private readonly AppDbContext _context;

    public CameraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CameraDevice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.CameraDevices.FindAsync([id], ct);

    public async Task<CameraDevice?> GetByCameraIdAsync(string cameraId, CancellationToken ct = default)
        => await _context.CameraDevices.FirstOrDefaultAsync(x => x.CameraId == cameraId, ct);

    public async Task<List<CameraDevice>> GetActiveAsync(CancellationToken ct = default)
        => await _context.CameraDevices.Where(x => x.Active).ToListAsync(ct);

    public async Task<List<CameraDevice>> ListByAreaAsync(string areaCode, CancellationToken ct = default)
        => await _context.CameraDevices.Where(x => x.AreaCode == areaCode).ToListAsync(ct);

    public async Task<CameraDevice> AddAsync(CameraDevice camera, CancellationToken ct = default)
    {
        _context.CameraDevices.Add(camera);
        await _context.SaveChangesAsync(ct);
        return camera;
    }

    public async Task<CameraDevice> UpdateAsync(CameraDevice camera, CancellationToken ct = default)
    {
        _context.CameraDevices.Update(camera);
        await _context.SaveChangesAsync(ct);
        return camera;
    }
        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.CameraDevices.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        _context.CameraDevices.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}