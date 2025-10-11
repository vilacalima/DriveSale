using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<List<Vehicle>> ListAvailableAsync(CancellationToken ct = default);
    Task<List<Vehicle>> ListSoldAsync(CancellationToken ct = default);
}

