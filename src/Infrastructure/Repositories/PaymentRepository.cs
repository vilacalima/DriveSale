using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PaymentRepository : EfRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext db) : base(db) { }

    public async Task<Payment?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _db.Payments.FirstOrDefaultAsync(p => p.Code == code, ct);
}

