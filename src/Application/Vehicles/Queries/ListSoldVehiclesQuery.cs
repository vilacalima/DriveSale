using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Vehicles.Queries;

public record ListSoldVehiclesQuery() : IRequest<IReadOnlyList<Vehicle>>;

public class ListSoldVehiclesQueryHandler(IVehicleRepository vehicleRepository, ILogger<ListSoldVehiclesQueryHandler> logger) : IRequestHandler<ListSoldVehiclesQuery, IReadOnlyList<Vehicle>>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ILogger<ListSoldVehiclesQueryHandler> _logger = logger;

    public async Task<IReadOnlyList<Vehicle>> Handle(ListSoldVehiclesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Listing sold vehicles");
            return await _vehicleRepository.ListSoldAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error listing sold vehicles");
            throw;
        }
    }
}
