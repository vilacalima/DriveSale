using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Vehicles.Commands;

public record UpdateVehicleCommand(Guid Id, string Brand, string Model, int Year, string Color, decimal Price) : IRequest<Unit>;

public class UpdateVehicleCommandHandler(IVehicleRepository vehicleRepository, IUnitOfWork uow, ILogger<UpdateVehicleCommandHandler> logger) : IRequestHandler<UpdateVehicleCommand, Unit>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<UpdateVehicleCommandHandler> _logger = logger;

    public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Update vehicle {Id}", request.Id);
            var entity = await _vehicleRepository.GetByIdAsync(request.Id, cancellationToken)
                                 ?? throw new KeyNotFoundException("Veículo não encontrado");

            entity.Update(request.Brand, request.Model, request.Year, request.Color, request.Price);
            _vehicleRepository.Update(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle {Id}", request.Id);
            throw;
        }
    }
}
