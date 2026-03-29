namespace MiniMercadoInteligente.Domain.Ports;

public interface IGatePort
{
    Task OpenAsync(string gateId, CancellationToken ct = default);

    Task CloseAsync(string gateId, CancellationToken ct = default);
}