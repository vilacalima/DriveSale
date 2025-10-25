using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Commands;

public record ApplyPaymentWebhookCommand(string PaymentCode, string Status, string? Provider = null) : IRequest<Sale?>;

public class ApplyPaymentWebhookCommandHandler(ISaleRepository saleRepository, IUnitOfWork uow, ILogger<ApplyPaymentWebhookCommandHandler> logger) : IRequestHandler<ApplyPaymentWebhookCommand, Sale?>
{
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<ApplyPaymentWebhookCommandHandler> _logger = logger;

    public async Task<Sale?> Handle(ApplyPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Apply payment webhook for code {Code} with status {Status}", request.PaymentCode, request.Status);
            Sale? sale = await GetByPaymentCodeAsync(request, cancellationToken);
            if (sale is null) return null;

            _logger.LogInformation("Validating webhook status");
            var normalized = request.Status?.Trim().ToLowerInvariant();

            var status = normalized switch
            {
                "paid" => PaymentStatus.Paid,
                "canceled" => PaymentStatus.Canceled,
                _ => (PaymentStatus?)null
            };

            if (status is null)
                throw new ArgumentException("status deve ser 'paid' ou 'canceled'", nameof(request.Status));

            if (status == PaymentStatus.Paid)
                sale.MarkPaid();
            else
                sale.MarkCanceled();

            _logger.LogInformation("Persisting payment status change");

            await _uow.SaveChangesAsync(cancellationToken);
            return sale;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying payment webhook {Code}", request.PaymentCode);
            throw;
        }
    }

    private async Task<Sale?> GetByPaymentCodeAsync(ApplyPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Get sale by payment code {Code}", request.PaymentCode);
            return await _saleRepository.GetByPaymentCodeAsync(request.PaymentCode, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sale by payment code {Code}", request.PaymentCode);
            throw;
        }
    }

}
