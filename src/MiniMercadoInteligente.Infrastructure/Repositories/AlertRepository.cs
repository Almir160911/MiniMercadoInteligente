using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _context;

    public AlertRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Alert>> ListBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _context.Alerts
            .Where(x => x.SessionId == sessionId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Alert>> ListAsync(
        string? status,
        string? type,
        DateTime? from,
        DateTime? to,
        CancellationToken ct)
    {
        var query = _context.Alerts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<AlertStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(x => x.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(x => x.Type == type);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= to.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Alert?> GetAsync(Guid alertId, CancellationToken ct)
    {
        return await _context.Alerts.FindAsync([alertId], ct);
    }

    public async Task<Alert> AddAsync(Alert alert, CancellationToken ct)
    {
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync(ct);
        return alert;
    }

    public async Task<Alert> UpdateAsync(Alert alert, CancellationToken ct)
    {
        _context.Alerts.Update(alert);
        await _context.SaveChangesAsync(ct);
        return alert;
    }
}