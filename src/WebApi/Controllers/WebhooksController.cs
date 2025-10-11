using Application.Payments.DTOs;
using Application.Payments.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public WebhooksController(IMediator mediator) => _mediator = mediator;

    [HttpPost("payments/{paymentCode}")]
    public async Task<IActionResult> PaymentUpdate([FromRoute] string paymentCode, [FromBody] PaymentWebhookDto dto, CancellationToken ct)
    {
        try
        {
            var sale = await _mediator.Send(new ApplyPaymentWebhookCommand(paymentCode, dto.Status, dto.Provider), ct);
            if (sale is null) return NotFound();
            return Accepted();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
