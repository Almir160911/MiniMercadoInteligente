using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/admin/gates")]
public class AdminGatesController : ControllerBase
{
    private readonly IAdminPanelService _service;

    public AdminGatesController(IAdminPanelService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<GateAdminResponse>> List(CancellationToken ct) => _service.ListGatesAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await _service.GetGateAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public Task<GateAdminResponse> Create([FromBody] CreateGateAdminRequest req, CancellationToken ct)
        => _service.CreateGateAsync(req, ct);

    [HttpPut("{id:guid}")]
    public Task<GateAdminResponse> Update(Guid id, [FromBody] UpdateGateAdminRequest req, CancellationToken ct)
        => _service.UpdateGateAsync(id, req, ct);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _service.DeleteGateAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}