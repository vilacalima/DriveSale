using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Payment : EntityBase, IAggregateRoot
{
    public string Code { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? Provider { get; private set; }

    private Payment() { }

    public Payment(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Amount = amount;
        Code = Guid.NewGuid().ToString("N");
    }

    public void ApplyStatus(PaymentStatus status, string? provider = null)
    {
        // idempotência básica
        if (Status == status) return;
        Status = status;
        Provider = provider ?? Provider;
        Touch();
    }
}

