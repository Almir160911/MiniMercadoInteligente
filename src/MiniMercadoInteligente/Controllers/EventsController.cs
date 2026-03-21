using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventsController : ControllerBase
{
    private readonly IEventIngestService _service;

    public EventsController(IEventIngestService service)
    {
        _service = service;
    }

    [HttpPost("ingest")]
    public async Task<IActionResult> Ingest(
        [FromBody] IngestEventRequest request,
        CancellationToken ct)
    {
        await _service.IngestAsync(request, ct);
        return Ok();
    }
}