using Domain.Abstractions;

namespace Application.Common.Interfaces;

public interface IRepository<T> where T : EntityBase, IAggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}

