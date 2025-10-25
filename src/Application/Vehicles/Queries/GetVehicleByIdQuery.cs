using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Vehicles.Queries;

public record GetVehicleByIdQuery(Guid Id) : IRequest<Vehicle?>;

public class GetVehicleByIdQueryHandler(IVehicleRepository vehicleRepository, ILogger<GetVehicleByIdQueryHandler> logger) : IRequestHandler<GetVehicleByIdQuery, Vehicle?>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ILogger<GetVehicleByIdQueryHandler> _logger = logger;

    public Task<Vehicle?> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Get vehicle by id {Id}", request.Id);
            return _vehicleRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vehicle by id {Id}", request.Id);
            throw;
        }
    }
}
