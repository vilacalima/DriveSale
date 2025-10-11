using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Vehicles.Queries;

public record GetVehicleByIdQuery(Guid Id) : IRequest<Vehicle?>;

public class GetVehicleByIdQueryHandler(IVehicleRepository vehicleRepository) : IRequestHandler<GetVehicleByIdQuery, Vehicle?>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

    public Task<Vehicle?> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[GetVehicleByIdQueryHandler][Handle] Get Veihicle By Id {request.Id}");
            return _vehicleRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetVehicleByIdQueryHandler][Handle] Error on execute {ex.Message}");
            throw;
        }
    }
}
