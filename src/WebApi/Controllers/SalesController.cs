using Application.Sales.DTOs;
using Application.Sales.Commands;
using Application.Sales.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaleCreateDto dto, CancellationToken ct)
    {
        try
        {
            var sale = await _mediator.Send(new CreateSaleCommand(dto.VehicleId, dto.BuyerCpf, dto.SaleDate), ct);
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, new
            {
                sale.Id,
                sale.TotalPrice,
                PaymentCode = sale.Payment.Code,
                PaymentStatus = sale.Payment.Status.ToString()
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Veículo não encontrado" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var sale = await _mediator.Send(new GetSaleByIdQuery(id), ct);
        if (sale is null) return NotFound();
        return Ok(sale);
    }
}
