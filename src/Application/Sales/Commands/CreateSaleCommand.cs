using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Commands;

public record CreateSaleCommand(Guid VehicleId, string BuyerCpf, DateTime? SaleDate = null) : IRequest<Sale>;

public class CreateSaleCommandHandler(IVehicleRepository vehicleRepository, ISaleRepository saleRepository, IUnitOfWork uow, IClientRepository clientRepository, ILogger<CreateSaleCommandHandler> logger)
    : IRequestHandler<CreateSaleCommand, Sale>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<CreateSaleCommandHandler> _logger = logger;

    public async Task<Sale> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating sale for vehicle {VehicleId}", request.VehicleId);
            var vehicle = await GetVehicle(request, cancellationToken);

            if (vehicle.Status == VehicleStatus.Sold)
                throw new InvalidOperationException("Veículo já vendido");

            var cpf = new Cpf(request.BuyerCpf);
            var client = await GetClient(cpf, cancellationToken)
                ?? throw new Exception("Cliente não encontrado, não foi possível seguir com a operação");

            var sale = new Sale(vehicle, client);

            if (request.SaleDate.HasValue)
                sale.SetSaleDate(request.SaleDate.Value);

            _logger.LogInformation("Persisting new sale");

            await _saleRepository.AddAsync(sale, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return sale;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale for vehicle {VehicleId}", request.VehicleId);
            throw;
        }
    }

    private async Task<Vehicle> GetVehicle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching vehicle by id {VehicleId}", request.VehicleId);

            return await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken)
                   ?? throw new KeyNotFoundException("Veículo não encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vehicle {VehicleId}", request.VehicleId);
            throw;
        }
    }

    private async Task<Client?> GetClient(Cpf cpf, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching client by CPF {Cpf}", cpf);
            return await _clientRepository.GetByCpfAsync(cpf, cancellationToken)
                   ?? throw new KeyNotFoundException("Cliente não encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching client by CPF {Cpf}", cpf);
            throw;
        }
    }
}
