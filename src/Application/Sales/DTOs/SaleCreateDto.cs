namespace Application.Sales.DTOs;

public record SaleCreateDto(Guid VehicleId, string BuyerCpf, DateTime? SaleDate = null);

