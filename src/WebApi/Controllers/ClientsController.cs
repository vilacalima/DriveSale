using Application.Clients.DTOs;
using Application.Clients.Commands;
using Application.Clients.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClientCreateDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _mediator.Send(new CreateClientCommand(dto.Name, dto.Email, dto.Cpf), ct);
            if (created is null) return Problem(detail: "Falha ao criar cliente", statusCode: 500);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException)
        {
            return ValidationProblem("Dados invalidos");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ClientUpdateDto dto, CancellationToken ct)
    {
        try
        {
            await _mediator.Send(new UpdateClientCommand(id, dto.Name, dto.Email), ct);
            return NoContent();
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var client = await _mediator.Send(new GetClientByIdQuery(id), ct);
        if (client is null) return NotFound();
        return Ok(client);
    }
}
