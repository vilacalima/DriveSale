using Domain.Abstractions;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Sale : EntityBase, IAggregateRoot
{
    public Guid VehicleId { get; private set; }
    public Vehicle Vehicle { get; private set; } = default!;
    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = default!;
    public string BuyerCpf { get; private set; } = default!;
    public DateTime SaleDate { get; private set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; private set; }
    public Guid PaymentId { get; private set; }
    public Payment Payment { get; private set; } = default!;
    public bool Canceled { get; private set; }

    private Sale() { }

    public Sale(Vehicle vehicle, Cpf buyerCpf)
    {
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
        if (Vehicle.Status == VehicleStatus.Sold)
            throw new InvalidOperationException("Veiculo ja vendido");
        VehicleId = vehicle.Id;
        BuyerCpf = buyerCpf.Value;
        SaleDate = DateTime.UtcNow;
        TotalPrice = vehicle.Price;
        Payment = new Payment(TotalPrice);
        PaymentId = Payment.Id;
    }

    public Sale(Vehicle vehicle, Client client)
    {
        if (vehicle is null) throw new ArgumentNullException(nameof(vehicle));
        if (client is null) throw new ArgumentNullException(nameof(client));

        Vehicle = vehicle;
        if (Vehicle.Status == VehicleStatus.Sold)
            throw new InvalidOperationException("Veiculo ja vendido");
        VehicleId = vehicle.Id;
        Client = client;
        ClientId = client.Id;
        BuyerCpf = client.Cpf;
        SaleDate = DateTime.UtcNow;
        TotalPrice = vehicle.Price;
        Payment = new Payment(TotalPrice);
        PaymentId = Payment.Id;
    }

    public void SetSaleDate(DateTime saleDate)
    {
        SaleDate = saleDate;
    }

    public void MarkPaid()
    {
        Payment.ApplyStatus(PaymentStatus.Paid);
        Vehicle.MarkSold();
        Touch();
    }

    public void MarkCanceled()
    {
        Payment.ApplyStatus(PaymentStatus.Canceled);
        Vehicle.MarkAvailable();
        Canceled = true;
        Touch();
    }
}
