using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VehicleRepository : EfRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext db) : base(db) { }

    public async Task<List<Vehicle>> ListAvailableAsync(CancellationToken ct = default)
        => await _db.Vehicles.AsNoTracking()
            .Where(v => v.Status == VehicleStatus.Available)
            .OrderBy(v => v.Price)
            .ToListAsync(ct);

    public async Task<List<Vehicle>> ListSoldAsync(CancellationToken ct = default)
        => await _db.Vehicles.AsNoTracking()
            .Where(v => v.Status == VehicleStatus.Sold)
            .OrderBy(v => v.Price)
            .ToListAsync(ct);
}

