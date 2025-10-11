using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Vehicle : EntityBase, IAggregateRoot
{
    public string Brand { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public int Year { get; private set; }
    public string Color { get; private set; } = default!;
    public decimal Price { get; private set; }
    public VehicleStatus Status { get; private set; } = VehicleStatus.Available;

    private Vehicle() { }

    public Vehicle(string brand, string model, int year, string color, decimal price)
    {
        Update(brand, model, year, color, price);
    }

    public void Update(string brand, string model, int year, string color, decimal price)
    {
        if (Status == VehicleStatus.Sold)
            throw new InvalidOperationException("Não é possível editar veículo vendido.");
        if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Marca obrigatória", nameof(brand));
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Modelo obrigatório", nameof(model));
        if (year < 1950 || year > DateTime.UtcNow.Year + 1) throw new ArgumentOutOfRangeException(nameof(year));
        if (string.IsNullOrWhiteSpace(color)) throw new ArgumentException("Cor obrigatória", nameof(color));
        if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price));

        Brand = brand.Trim();
        Model = model.Trim();
        Year = year;
        Color = color.Trim();
        Price = price;
        Touch();
    }

    public void MarkSold()
    {
        if (Status == VehicleStatus.Sold)
            return;
        Status = VehicleStatus.Sold;
        Touch();
    }

    public void MarkAvailable()
    {
        if (Status == VehicleStatus.Available)
            return;
        Status = VehicleStatus.Available;
        Touch();
    }
}

