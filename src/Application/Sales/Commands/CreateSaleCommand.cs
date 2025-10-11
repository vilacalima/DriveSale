using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.Sales.Commands;

public record CreateSaleCommand(Guid VehicleId, string BuyerCpf, DateTime? SaleDate = null) : IRequest<Sale>;

public class CreateSaleCommandHandler(IVehicleRepository vehicleRepository, ISaleRepository saleRepository, IUnitOfWork uow, IClientRepository clientRepository) 
                : IRequestHandler<CreateSaleCommand, Sale>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Sale> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[CreateSaleCommandHandler][Handle] Init create sale {request}");
            Vehicle vehicle = await GetVehicle(request, cancellationToken);

            if (vehicle.Status == VehicleStatus.Sold)
                throw new InvalidOperationException("Veículo já vendido");

            var cpf = new Cpf(request.BuyerCpf);

            var client = GetClient(cpf, cancellationToken)
                ?? throw new Exception("Cliente não encontrado, não foi possivel seguir com a operação");

            var sale = new Sale(vehicle, cpf);

            if (request.SaleDate.HasValue)
                sale.SetSaleDate(request.SaleDate.Value);

            Console.WriteLine($"[CreateSaleCommandHandler][Handle] Save new sale");
            
            await _saleRepository.AddAsync(sale, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return sale;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetSaleByIdQueryHandler][Handle] Error on execute {ex.Message}");
            throw;
        }
    }

    private async Task<Vehicle> GetVehicle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[CreateSaleCommandHandler][GetVehicle] Get Vehicle by Id {request.VehicleId}");

            return await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken)
                                              ?? throw new KeyNotFoundException("Veículo não encontrado");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CreateSaleCommandHandler][GetVehicle] Error on execute {ex.Message}");
            throw;
        }
    }

    private async Task<Client?> GetClient(Cpf cpf, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[CreateSaleCommandHandler][GetClient] Get Client By Cpf {cpf}");
            return await _clientRepository.GetByCpfAsync(cpf, cancellationToken)
                        ?? throw new KeyNotFoundException("Cliente não encontrado");
                        ;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"[CreateSaleCommandHandler][GetClient] Error on execute {ex.Message}");
            throw;
        }
    }
}
