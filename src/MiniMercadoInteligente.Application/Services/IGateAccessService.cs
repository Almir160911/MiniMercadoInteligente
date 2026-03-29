namespace MiniMercadoInteligente.Application.Services;

public interface IGateAccessService
{
    Task<Guid> OpenEntryAsync(
        Guid residentId,
        Guid condominiumId,
        string deviceId,
        string gateId,
        CancellationToken ct = default);

    Task CloseExitAsync(
        Guid sessionId,
        Guid residentId,
        string gateId,
        CancellationToken ct = default);
}