using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }

    [HttpPost("{sessionId:guid}")]
    public async Task<ActionResult<PaymentDto>> Create(
        Guid sessionId,
        [FromBody] CreatePaymentRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(sessionId, request, ct);
        return Ok(result);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromBody] PaymentWebhookRequest request,
        CancellationToken ct)
    {
        await _service.HandleWebhookAsync(request, ct);
        return Ok();
    }
}