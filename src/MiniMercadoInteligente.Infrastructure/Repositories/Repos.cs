using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

#region CartRepository

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CartItem>> ListBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _context.CartItems
            .Where(x => x.SessionId == sessionId)
            .ToListAsync(ct);
    }

    public async Task<CartItem?> FindBySessionAndProductAsync(Guid sessionId, Guid productId, CancellationToken ct = default)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.ProductId == productId, ct);
    }

    public async Task AddAsync(CartItem item, CancellationToken ct)
    {
        _context.CartItems.Add(item);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CartItem item, CancellationToken ct = default)
    {
        _context.CartItems.Update(item);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid sessionId, Guid itemId, CancellationToken ct)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.CartItemId == itemId, ct);

        if (item is null) return;

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddOrIncrementItemAsync(Guid sessionId, Guid productId, int qty, decimal unitPrice, CancellationToken ct = default)
    {
        var existing = await FindBySessionAndProductAsync(sessionId, productId, ct);

        if (existing is null)
        {
            _context.CartItems.Add(new CartItem
            {
                CartItemId = Guid.NewGuid(),
                SessionId = sessionId,
                ProductId = productId,
                Qty = qty,
                OccurredAtUtc = DateTime.UtcNow,
                Source = "AUTO"
            });
        }
        else
        {
            existing.Qty += qty;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task DecrementOrRemoveItemAsync(Guid sessionId, Guid productId, int qty, CancellationToken ct = default)
    {
        var existing = await FindBySessionAndProductAsync(sessionId, productId, ct);
        if (existing is null) return;

        existing.Qty -= qty;

        if (existing.Qty <= 0)
            _context.CartItems.Remove(existing);

        await _context.SaveChangesAsync(ct);
    }

    public async Task<decimal> GetSessionTotalAsync(Guid sessionId, CancellationToken ct = default)
    {
        var items = await _context.CartItems
            .Where(x => x.SessionId == sessionId)
            .ToListAsync(ct);

        return items.Sum(x => x.Qty * 10m); // 🔥 ajustar depois com preço real
    }

    public async Task MarkAsCheckedOutAsync(Guid sessionId, CancellationToken ct = default)
    {
        // opcional: marcar flag futura
        await Task.CompletedTask;
    }

    public async Task MarkItemsAsReconciledAsync(Guid sessionId, CancellationToken ct = default)
    {
        await Task.CompletedTask;
    }
}

#endregion

#region SessionRepository

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _context;

    public SessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Session?> GetAsync(Guid sessionId, CancellationToken ct)
        => await _context.Sessions.FindAsync([sessionId], ct);

    public async Task<Session> AddAsync(Session session, CancellationToken ct)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync(ct);
        return session;
    }

    public async Task<Session> UpdateAsync(Session session, CancellationToken ct)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync(ct);
        return session;
    }

    public async Task<List<Session>> ListAsync(DateTime? from, DateTime? to, string? status, CancellationToken ct)
    {
        return await _context.Sessions.ToListAsync(ct);
    }

    public async Task<Session?> GetOpenByResidentAsync(Guid residentId, CancellationToken ct)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(x => x.ResidentId == residentId && x.Status == SessionStatus.Open, ct);
    }

    public async Task<Session?> GetOpenByDeviceAsync(string deviceId, CancellationToken ct)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(x => x.DeviceId == deviceId && x.Status == SessionStatus.Open, ct);
    }

    public async Task<Session?> GetOpenByTrackIdAsync(string trackId, CancellationToken ct = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(x => x.ActiveTrackId == trackId && x.Status == SessionStatus.Open, ct);
    }

    public async Task<List<Session>> ListOpenByAreaAsync(string areaCode, CancellationToken ct = default)
    {
        return await _context.Sessions
            .Where(x => x.CurrentAreaCode == areaCode && x.Status == SessionStatus.Open)
            .ToListAsync(ct);
    }

    public async Task<Session> CreateOpenSessionAsync(
        Guid residentId,
        Guid condominiumId,
        string deviceId,
        string entryMethod,
        string? entryGateId,
        CancellationToken ct = default)
    {
        var session = new Session
        {
            SessionId = Guid.NewGuid(),
            ResidentId = residentId,
            CondominiumId = condominiumId,
            DeviceId = deviceId,
            Status = SessionStatus.Open,
            StartedAt = DateTime.UtcNow,
            EnteredAtUtc = DateTime.UtcNow
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync(ct);

        return session;
    }

    public async Task CloseSessionAsync(Guid sessionId, DateTime endedAtUtc, DateTime? exitedAtUtc, CancellationToken ct = default)
    {
        var session = await GetAsync(sessionId, ct);
        if (session is null) return;

        session.Status = SessionStatus.Closed;
        session.EndedAt = endedAtUtc;
        session.ExitedAtUtc = exitedAtUtc;

        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateTrackingAsync(Guid sessionId, string? activeTrackId, string? currentAreaCode, decimal trackingConfidence, bool trackingLocked, CancellationToken ct = default)
    {
        var session = await GetAsync(sessionId, ct);
        if (session is null) return;

        session.ActiveTrackId = activeTrackId;
        session.CurrentAreaCode = currentAreaCode;

        await _context.SaveChangesAsync(ct);
    }

    public async Task MarkAutoCheckoutTriggeredAsync(Guid sessionId, DateTime triggeredAtUtc, CancellationToken ct = default)
    {
        await Task.CompletedTask;
    }

    public async Task BlockSessionAsync(Guid sessionId, string reason, CancellationToken ct = default)
    {
        var session = await GetAsync(sessionId, ct);
        if (session is null) return;

        session.Status = SessionStatus.Blocked;
        await _context.SaveChangesAsync(ct);
    }
}

#endregion

#region PaymentRepository

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken ct = default)
        => await _context.Payments.FindAsync([paymentId], ct);

    public async Task<Payment?> GetLastBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _context.Payments
            .Where(x => x.SessionId == sessionId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Payment?> GetLastPendingBySessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        return await _context.Payments
            .Where(x => x.SessionId == sessionId && x.Status == PaymentStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<Payment>> ListBySessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        return await _context.Payments
            .Where(x => x.SessionId == sessionId)
            .ToListAsync(ct);
    }

    public async Task<Payment?> GetLastByResidentAsync(Guid residentId, CancellationToken ct = default)
    {
        return await _context.Payments
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddAsync(Payment payment, CancellationToken ct)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Payment payment, CancellationToken ct)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Payment?> FindByWebhookAsync(Guid? paymentId, string? gatewayRef, CancellationToken ct)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(x =>
                (paymentId != null && x.PaymentId == paymentId) ||
                (gatewayRef != null && x.GatewayRef == gatewayRef), ct);
    }

    public async Task<Payment> CreateAuthorizedChargeAsync(Guid sessionId, Guid residentId, decimal amount, string? token, CancellationToken ct = default)
    {
        var payment = new Payment
        {
            PaymentId = Guid.NewGuid(),
            SessionId = sessionId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(ct);

        return payment;
    }

    public async Task IncrementRetryCountAsync(Guid paymentId, CancellationToken ct = default)
    {
        await Task.CompletedTask;
    }

    public async Task MarkRequiresReviewAsync(Guid paymentId, string reason, CancellationToken ct = default)
    {
        await Task.CompletedTask;
    }

    public async Task<List<Payment>> ListPendingAutoCheckoutAsync(CancellationToken ct = default)
    {
        return await _context.Payments
            .Where(x => x.Status == PaymentStatus.Pending)
            .ToListAsync(ct);
    }
}

#endregion