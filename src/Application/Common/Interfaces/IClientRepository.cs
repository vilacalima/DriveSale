using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetByCpfAsync(string cpf, CancellationToken ct = default);
}

