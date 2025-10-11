using Application.Common.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Clients.Commands;

public record CreateClientCommand(string Name, string Email, string Cpf) : IRequest<Client>;

public class CreateClientCommandHandler(IClientRepository clientRepository, IUnitOfWork uow)
            : IRequestHandler<CreateClientCommand, Client?>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Client?> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Cpf cpf = new Cpf(request.Cpf);
            var entity = new Client(request.Name, request.Email, cpf);

            await _clientRepository.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return await GetClient(cpf, cancellationToken);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CreateClientCommandHandler] Erro on execute {ex.Message}");
            throw;
        }
    }

    private async Task<Client?> GetClient(Cpf cpf, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[CreateClientCommandHandler][GetClient] Get Client By Cpf {cpf}");
            return await _clientRepository.GetByCpfAsync(cpf, cancellationToken);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"[CreateClientCommandHandler][GetClient] Error on execute {ex.Message}");
            throw;
        }

    }
}

