using System;

namespace Domain.Abstractions;

public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    public void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

