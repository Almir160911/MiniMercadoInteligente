using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/gates")]
public class GatesController : ControllerBase
{
    private readonly IGateAccessService _gateAccessService;
    private readonly ISessionService _sessionService;

    public GatesController(
        IGateAccessService gateAccessService,
        ISessionService sessionService)
    {
        _gateAccessService = gateAccessService;
        _sessionService = sessionService;
    }

    [HttpPost("entry")]
    public async Task<IActionResult> Entry([FromBody] GateEntryRequest request, CancellationToken ct)
    {
        var sessionId = await _gateAccessService.OpenEntryAsync(
            request.ResidentId,
            request.CondominiumId,
            request.DeviceId,
            request.GateId,
            ct);

        var detail = await _sessionService.GetDetailAsync(sessionId, ct);

        return Ok(new
        {
            SessionId = sessionId,
            Session = detail
        });
    }

    [HttpPost("exit")]
    public async Task<IActionResult> Exit([FromBody] GateExitRequest request, CancellationToken ct)
    {
        await _gateAccessService.CloseExitAsync(
            request.SessionId,
            request.ResidentId,
            request.GateId,
            ct);

        var detail = await _sessionService.GetDetailAsync(request.SessionId, ct);

        return Ok(new
        {
            SessionId = request.SessionId,
            Session = detail
        });
    }

    [HttpGet("sessions/{sessionId:guid}")]
    public async Task<IActionResult> GetSession(Guid sessionId, CancellationToken ct)
    {
        var detail = await _sessionService.GetDetailAsync(sessionId, ct);

        if (detail is null)
            return NotFound(new { message = "Sessão não encontrada." });

        return Ok(detail);
    }
}

public sealed class GateEntryRequest
{
    public Guid ResidentId { get; set; }
    public Guid CondominiumId { get; set; }
    public string DeviceId { get; set; } = default!;
    public string GateId { get; set; } = default!;
}

public sealed class GateExitRequest
{
    public Guid SessionId { get; set; }
    public Guid ResidentId { get; set; }
    public string GateId { get; set; } = default!;
}