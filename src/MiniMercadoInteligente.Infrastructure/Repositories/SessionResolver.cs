using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class SessionResolver : ISessionResolver
{
    private readonly ISessionRepository _sessions;

    public SessionResolver(ISessionRepository sessions)
    {
        _sessions = sessions;
    }

    public async Task<Guid?> ResolveSessionByTrackAsync(string trackId, string areaCode, CancellationToken ct = default)
    {
        var session = await _sessions.GetOpenByTrackIdAsync(trackId, ct);

        if (session is not null)
            return session.SessionId;

        var sessionsInArea = await _sessions.ListOpenByAreaAsync(areaCode, ct);

        return sessionsInArea.FirstOrDefault()?.SessionId;
    }
}