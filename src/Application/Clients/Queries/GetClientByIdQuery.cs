using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Clients.Queries;

public record GetClientByIdQuery(Guid Id) : IRequest<Client?>;

public class GetClientByIdQueryHandler(IClientRepository clientRepository, ILogger<GetClientByIdQueryHandler> logger) 
            : IRequestHandler<GetClientByIdQuery, Client?>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly ILogger<GetClientByIdQueryHandler> _logger = logger;
    public async Task<Client?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
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
