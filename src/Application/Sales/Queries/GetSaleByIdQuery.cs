using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Sales.Queries;

public record GetSaleByIdQuery(Guid Id) : IRequest<Sale?>;

public class GetSaleByIdQueryHandler(ISaleRepository saleRepository) : IRequestHandler<GetSaleByIdQuery, Sale?>
{
    private readonly ISaleRepository _saleRepository = saleRepository;

    public Task<Sale?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[GetSaleByIdQueryHandler][Handle] Get Sale by Id {request.Id}");
            return _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetSaleByIdQueryHandler][Handle] Error on execute {ex.Message}");
            throw;
        }
    }
}
