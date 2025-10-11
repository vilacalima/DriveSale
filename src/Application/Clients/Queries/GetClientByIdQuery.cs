using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Clients.Queries;

public record GetClientByIdQuery(Guid Id) : IRequest<Client?>;

public class GetClientByIdQueryHandler(IClientRepository clientRepository) 
            : IRequestHandler<GetClientByIdQuery, Client?>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    public async Task<Client?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[GetClientByIdQueryHandler][Handle] Get Client by Id {request.Id}");
            return await _clientRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetClientByIdQueryHandler][Handle] Error on Execute {ex}");
            throw;
        }
    }
}

