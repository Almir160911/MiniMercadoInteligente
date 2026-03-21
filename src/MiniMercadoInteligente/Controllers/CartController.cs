using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/v1/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service)
    {
        _service = service;
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<CartResponse>> Get(Guid sessionId, CancellationToken ct)
    {
        var result = await _service.GetAsync(sessionId, ct);
        return Ok(result);
    }

    [HttpPost("{sessionId:guid}/items")]
    public async Task<ActionResult<CartItemDto>> Add(
        Guid sessionId,
        [FromBody] AddCartItemRequest request,
        CancellationToken ct)
    {
        var result = await _service.AddAsync(sessionId, request, ct);
        return Ok(result);
    }

    [HttpDelete("{sessionId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> Remove(Guid sessionId, Guid itemId, CancellationToken ct)
    {
        await _service.RemoveAsync(sessionId, itemId, ct);
        return NoContent();
    }
}