using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Clients.Commands;

public record UpdateClientCommand(Guid Id, string Name, string Email) : IRequest<Client?>;

public class UpdateClientCommandHandler(IClientRepository clientRepository, IUnitOfWork uow)
            : IRequestHandler<UpdateClientCommand, Client?>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Client?> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        Client? entity = await GetClientByIdAsync(request, cancellationToken);
        if (entity is null) return null;

        try
        {
            Console.WriteLine($"[UpdateClientCommandHandler][Handle] Update client {request.Id}");

            entity.Update(request.Name, request.Email);
            _clientRepository.Update(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpdateClientCommandHandler][Handle] Error on Execute {ex}");
            throw;
        }
    }

    private async Task<Client?> GetClientByIdAsync(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[UpdateClientCommandHandler][GetClientByIdAsync] Get client by Id {request.Id}");
            return await _clientRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpdateClientCommandHandler][GetClientByIdAsync] Error on Execute {ex}");
            throw;
        }
        
    }

}

