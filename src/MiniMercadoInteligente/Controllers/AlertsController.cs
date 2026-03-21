using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/v1/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _service;

    public AlertsController(IAlertService service)
    {
        _service = service;
    }

    [HttpGet("session/{sessionId:guid}")]
    public async Task<ActionResult<List<AlertDto>>> ListBySession(Guid sessionId, CancellationToken ct)
    {
        var result = await _service.ListAsync(sessionId, ct);
        return Ok(result);
    }

    [HttpPost("{alertId:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid alertId, [FromBody] ResolveAlertRequest request, CancellationToken ct)
    {
        var ok = await _service.ResolveAsync(alertId, request, ct);
        return ok ? Ok() : NotFound();
    }
}