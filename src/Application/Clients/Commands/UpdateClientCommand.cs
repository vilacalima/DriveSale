using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Clients.Commands;

public record UpdateClientCommand(Guid Id, string Name, string Email) : IRequest<Client?>;

public class UpdateClientCommandHandler(IClientRepository clientRepository, IUnitOfWork uow, ILogger<UpdateClientCommandHandler> logger)
            : IRequestHandler<UpdateClientCommand, Client?>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<UpdateClientCommandHandler> _logger = logger;

    public async Task<Client?> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        Client? entity = await GetClientByIdAsync(request, cancellationToken);
        if (entity is null) return null;

        try
        {
            _logger.LogInformation("Update client {Id}", request.Id);

            entity.Update(request.Name, request.Email);
            _clientRepository.Update(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client {Id}", request.Id);
            throw;
        }
    }

    private async Task<Client?> GetClientByIdAsync(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Get client by Id {Id}", request.Id);
            return await _clientRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client by Id {Id}", request.Id);
            throw;
        }
        
    }

}
