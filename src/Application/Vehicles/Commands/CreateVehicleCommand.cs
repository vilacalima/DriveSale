using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Vehicles.Commands;

public record CreateVehicleCommand(string Brand, string Model, int Year, string Color, decimal Price) : IRequest<Vehicle>;

public class CreateVehicleCommandHandler(IVehicleRepository vehicleRepository, IUnitOfWork uow) : IRequestHandler<CreateVehicleCommand, Vehicle>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Vehicle> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[CreateVehicleCommandHandler][Handle] Create Vehicle {request}");
            var entity = new Vehicle(request.Brand, request.Model, request.Year, request.Color, request.Price);
            await _vehicleRepository.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CreateVehicleCommandHandler][Handle] Error on execute {ex.Message}");
            throw;
        }
    }
}
