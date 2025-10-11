using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SaleRepository : EfRepository<Sale>, ISaleRepository
{
    public SaleRepository(AppDbContext db) : base(db) { }

    public async Task<Sale?> GetByPaymentCodeAsync(string code, CancellationToken ct = default)
        => await _db.Sales.Include(s => s.Payment).Include(s => s.Vehicle)
               .FirstOrDefaultAsync(s => s.Payment.Code == code, ct);
}

