using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Vehicles.Queries;

public record ListAvailableVehiclesQuery() : IRequest<IReadOnlyList<Vehicle>>;

public class ListAvailableVehiclesQueryHandler(IVehicleRepository vehicleRepository, ILogger<ListAvailableVehiclesQueryHandler> logger) : IRequestHandler<ListAvailableVehiclesQuery, IReadOnlyList<Vehicle>>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ILogger<ListAvailableVehiclesQueryHandler> _logger = logger;

    public async Task<IReadOnlyList<Vehicle>> Handle(ListAvailableVehiclesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Listing available vehicles");
            return await _vehicleRepository.ListAvailableAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing available vehicles");
            throw;
        }
    }
}
