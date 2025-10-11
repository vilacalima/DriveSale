using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByCodeAsync(string code, CancellationToken ct = default);
}

