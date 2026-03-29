using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Application.Services;

public class GateAccessService : IGateAccessService
{
    private readonly ISessionRepository _sessions;
    private readonly IGatePort _gatePort;
    private readonly IAutoCheckoutService _checkout;

    public GateAccessService(
        ISessionRepository sessions,
        IGatePort gatePort,
        IAutoCheckoutService checkout)
    {
        _sessions = sessions;
        _gatePort = gatePort;
        _checkout = checkout;
    }

    public async Task<Guid> OpenEntryAsync(
        Guid residentId,
        Guid condominiumId,
        string deviceId,
        string gateId,
        CancellationToken ct = default)
    {
        var session = await _sessions.CreateOpenSessionAsync(
            residentId,
            condominiumId,
            deviceId,
            "GATE",
            gateId,
            ct);

        await _gatePort.OpenAsync(gateId, ct);

        return session.SessionId;
    }
public async Task CloseExitAsync(
    Guid sessionId,
    Guid residentId,
    string gateId,
    CancellationToken ct = default)
{
    await _checkout.CheckoutAsync(sessionId, residentId, ct);

    var session = await _sessions.GetAsync(sessionId, ct)
        ?? throw new InvalidOperationException("Sessão não encontrada.");

    if (session.Status == SessionStatus.Blocked)
        throw new InvalidOperationException("Saída bloqueada por regra antifraude.");

    await _gatePort.OpenAsync(gateId, ct);

    await _sessions.CloseSessionAsync(
        sessionId,
        DateTime.UtcNow,
        DateTime.UtcNow,
        ct);
}

}