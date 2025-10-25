using Application.Common.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Clients.Commands;

public record CreateClientCommand(string Name, string Email, string Cpf) : IRequest<Client>;

public class CreateClientCommandHandler(IClientRepository clientRepository, IUnitOfWork uow, ILogger<CreateClientCommandHandler> logger)
            : IRequestHandler<CreateClientCommand, Client?>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<CreateClientCommandHandler> _logger = logger;

    public async Task<Client?> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating client {Email}", request.Email);
            Cpf cpf = new Cpf(request.Cpf);
            var entity = new Client(request.Name, request.Email, cpf);

            await _clientRepository.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return await GetClient(cpf, cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client {Email}", request.Email);
            throw;
        }
    }

    private async Task<Client?> GetClient(Cpf cpf, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Get client by CPF {Cpf}", cpf);
            return await _clientRepository.GetByCpfAsync(cpf, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error getting client by CPF {Cpf}", cpf);
            throw;
        }

    }
}
