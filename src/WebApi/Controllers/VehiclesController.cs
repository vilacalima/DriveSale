using Application.Vehicles.DTOs;
using Application.Vehicles.Commands;
using Application.Vehicles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VehicleCreateDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _mediator.Send(new CreateVehicleCommand(dto.Brand, dto.Model, dto.Year, dto.Color, dto.Price), ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException)
        {
            return ValidationProblem("Dados invalidos");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var v = await _mediator.Send(new GetVehicleByIdQuery(id), ct);
        if (v is null) return NotFound();
        return Ok(v);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] VehicleUpdateDto dto, CancellationToken ct)
    {
        try
        {
            await _mediator.Send(new UpdateVehicleCommand(id, dto.Brand, dto.Model, dto.Year, dto.Color, dto.Price), ct);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { message = "Nao e possivel editar veiculo vendido." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return ValidationProblem("Dados invalidos");
        }
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string status = "available", CancellationToken ct = default)
    {
        return status.ToLowerInvariant() switch
        {
            "available" => Ok(await _mediator.Send(new ListAvailableVehiclesQuery(), ct)),
            "sold" => Ok(await _mediator.Send(new ListSoldVehiclesQuery(), ct)),
            _ => BadRequest(new { message = "status deve ser 'available' ou 'sold'" })
        };
    }
}
