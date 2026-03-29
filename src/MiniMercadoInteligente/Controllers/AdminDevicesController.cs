using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/admin/devices")]
public class AdminDevicesController : ControllerBase
{
    private readonly IAdminPanelService _service;

    public AdminDevicesController(IAdminPanelService service)
    {
        _service = service;
    }

    [HttpPost("api-key")]
    public Task<DeviceApiKeyAdminResponse> UpsertApiKey([FromBody] CreateOrUpdateDeviceApiKeyAdminRequest req, CancellationToken ct)
        => _service.UpsertDeviceApiKeyAsync(req, ct);

    [HttpDelete("api-key/{deviceApiKeyId:guid}")]
    public async Task<IActionResult> DeleteApiKey(Guid deviceApiKeyId, CancellationToken ct)
    {
        var ok = await _service.DeleteDeviceApiKeyAsync(deviceApiKeyId, ct);
        return ok ? NoContent() : NotFound();
    }
}