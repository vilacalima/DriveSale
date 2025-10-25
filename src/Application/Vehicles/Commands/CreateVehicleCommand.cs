using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Vehicles.Commands;

public record CreateVehicleCommand(string Brand, string Model, int Year, string Color, decimal Price) : IRequest<Vehicle>;

public class CreateVehicleCommandHandler(IVehicleRepository vehicleRepository, IUnitOfWork uow, ILogger<CreateVehicleCommandHandler> logger) : IRequestHandler<CreateVehicleCommand, Vehicle>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<CreateVehicleCommandHandler> _logger = logger;

    public async Task<Vehicle> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Create vehicle {@Request}", request);
            var entity = new Vehicle(request.Brand, request.Model, request.Year, request.Color, request.Price);
            await _vehicleRepository.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle");
            throw;
        }
    }
}
