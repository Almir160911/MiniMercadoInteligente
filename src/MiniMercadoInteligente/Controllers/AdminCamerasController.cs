using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/admin/cameras")]
public class AdminCamerasController : ControllerBase
{
    private readonly IAdminPanelService _service;

    public AdminCamerasController(IAdminPanelService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<CameraDeviceAdminResponse>> List(CancellationToken ct) => _service.ListCamerasAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await _service.GetCameraAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public Task<CameraDeviceAdminResponse> Create([FromBody] CreateCameraDeviceAdminRequest req, CancellationToken ct)
        => _service.CreateCameraAsync(req, ct);

    [HttpPut("{id:guid}")]
    public Task<CameraDeviceAdminResponse> Update(Guid id, [FromBody] UpdateCameraDeviceAdminRequest req, CancellationToken ct)
        => _service.UpdateCameraAsync(id, req, ct);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _service.DeleteCameraAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}