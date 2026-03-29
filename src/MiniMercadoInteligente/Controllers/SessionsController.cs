using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _service;

    public SessionsController(ISessionService service)
    {
        _service = service;
    }

    [HttpPost("open")]
    public async Task<ActionResult<CreateSessionResponse>> Open(
        [FromBody] CreateSessionRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<SessionDetailResponse>> Get(
        Guid sessionId,
        CancellationToken ct)
    {
        var result = await _service.GetDetailAsync(sessionId, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{sessionId:guid}/close")]
    public async Task<ActionResult<CloseSessionResponse>> Close(
        Guid sessionId,
        [FromBody] CloseSessionRequest request,
        CancellationToken ct)
    {
        var result = await _service.CloseAsync(sessionId, request, ct);
        return Ok(result);
    }
}