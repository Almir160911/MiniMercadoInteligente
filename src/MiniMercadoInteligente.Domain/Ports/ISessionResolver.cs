namespace MiniMercadoInteligente.Domain.Ports;

public interface ISessionResolver
{
    Task<Guid?> ResolveSessionByTrackAsync(
        string trackId,
        string areaCode,
        CancellationToken ct = default);
}