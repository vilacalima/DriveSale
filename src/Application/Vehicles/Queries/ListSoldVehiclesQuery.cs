using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Vehicles.Queries;

public record ListSoldVehiclesQuery() : IRequest<IReadOnlyList<Vehicle>>;

public class ListSoldVehiclesQueryHandler(IVehicleRepository vehicleRepository) : IRequestHandler<ListSoldVehiclesQuery, IReadOnlyList<Vehicle>>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

    public async Task<IReadOnlyList<Vehicle>> Handle(ListSoldVehiclesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("[ListSoldVehiclesQueryHandler][Handle] List Sold Vehicles");
            return await _vehicleRepository.ListSoldAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"[ListSoldVehiclesQueryHandler][Handle] Error on execute {ex.Message}");
            throw;
        }
    }
}
