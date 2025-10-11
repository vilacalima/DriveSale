namespace Application.Vehicles.DTOs;

public record VehicleCreateDto(string Brand, string Model, int Year, string Color, decimal Price);

