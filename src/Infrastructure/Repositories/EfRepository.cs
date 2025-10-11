using Application.Common.Interfaces;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class EfRepository<T> : IRepository<T> where T : EntityBase, IAggregateRoot
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public EfRepository(AppDbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        => await _set.AddAsync(entity, ct);

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _set.FindAsync([id], ct);

    public virtual void Remove(T entity) => _set.Remove(entity);
    public virtual void Update(T entity) => _set.Update(entity);
}

