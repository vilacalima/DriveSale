using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Vehicles.Queries;

public record ListAvailableVehiclesQuery() : IRequest<IReadOnlyList<Vehicle>>;

public class ListAvailableVehiclesQueryHandler(IVehicleRepository vehicleRepository) : IRequestHandler<ListAvailableVehiclesQuery, IReadOnlyList<Vehicle>>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

    public async Task<IReadOnlyList<Vehicle>> Handle(ListAvailableVehiclesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("[ListAvailableVehiclesQuery][Handle] List Available Veicles Query");
            return await _vehicleRepository.ListAvailableAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ListAvailableVehiclesQuery][Handle] Error on execute {ex.Message}");
            throw;
        }
    }
}
