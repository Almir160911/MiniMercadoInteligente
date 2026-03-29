using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/admin/sensors")]
public class AdminSensorsController : ControllerBase
{
    private readonly IAdminPanelService _service;

    public AdminSensorsController(IAdminPanelService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<WeightSensorAdminResponse>> List(CancellationToken ct) => _service.ListSensorsAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await _service.GetSensorAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public Task<WeightSensorAdminResponse> Create([FromBody] CreateWeightSensorAdminRequest req, CancellationToken ct)
        => _service.CreateSensorAsync(req, ct);

    [HttpPut("{id:guid}")]
    public Task<WeightSensorAdminResponse> Update(Guid id, [FromBody] UpdateWeightSensorAdminRequest req, CancellationToken ct)
        => _service.UpdateSensorAsync(id, req, ct);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _service.DeleteSensorAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}