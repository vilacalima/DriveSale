using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetByPaymentCodeAsync(string code, CancellationToken ct = default);
}

