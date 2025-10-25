using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Queries;

public record GetSaleByIdQuery(Guid Id) : IRequest<Sale?>;

public class GetSaleByIdQueryHandler(ISaleRepository saleRepository, ILogger<GetSaleByIdQueryHandler> logger) : IRequestHandler<GetSaleByIdQuery, Sale?>
{
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly ILogger<GetSaleByIdQueryHandler> _logger = logger;

    public Task<Sale?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Get sale by Id {Id}", request.Id);
            return _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sale by Id {Id}", request.Id);
            throw;
        }
    }
}
