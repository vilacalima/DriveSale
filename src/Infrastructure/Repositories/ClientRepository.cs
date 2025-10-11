using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClientRepository : EfRepository<Client>, IClientRepository
{
    public ClientRepository(AppDbContext db) : base(db) { }

    public async Task<Client?> GetByCpfAsync(string cpf, CancellationToken ct = default)
        => await _db.Clients.FirstOrDefaultAsync(c => c.Cpf == cpf, ct);
}

