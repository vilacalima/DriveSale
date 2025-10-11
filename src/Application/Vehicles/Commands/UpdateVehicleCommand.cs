using Application.Common.Interfaces;
using MediatR;

namespace Application.Vehicles.Commands;

public record UpdateVehicleCommand(Guid Id, string Brand, string Model, int Year, string Color, decimal Price) : IRequest<Unit>;

public class UpdateVehicleCommandHandler(IVehicleRepository vehicleRepository, IUnitOfWork uow) : IRequestHandler<UpdateVehicleCommand, Unit>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[UpdateVehicleCommandHandler][Handle] Update Vehicle {request}");
            var entity = await _vehicleRepository.GetByIdAsync(request.Id, cancellationToken)
                                 ?? throw new KeyNotFoundException("Veículo não encontrado");

            entity.Update(request.Brand, request.Model, request.Year, request.Color, request.Price);
            _vehicleRepository.Update(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpdateVehicleCommandHandler][Handle] Error on execute {ex.Message}");
            throw;
        }
    }
}
