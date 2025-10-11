using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Payments.Commands;

public record ApplyPaymentWebhookCommand(string PaymentCode, string Status, string? Provider = null) : IRequest<Sale?>;

public class ApplyPaymentWebhookCommandHandler(ISaleRepository saleRepository, IUnitOfWork uow) : IRequestHandler<ApplyPaymentWebhookCommand, Sale?>
{
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Sale?> Handle(ApplyPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[ApplyPaymentWebhookCommandHandler][Handle] Init Apply Payment Webhook {request}");
            Sale? sale = await GetByPaymentCodeAsync(request, cancellationToken);
            if (sale is null) return null;

            Console.WriteLine($"[ApplyPaymentWebhookCommandHandler][Handle] Valid Status");
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

            Console.WriteLine($"[ApplyPaymentWebhookCommandHandler][Handle] Save Changes");

            await _uow.SaveChangesAsync(cancellationToken);
            return sale;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApplyPaymentWebhookCommandHandler][Handle] Error on execute {ex.Message}");
            throw;
        }
    }

    private async Task<Sale?> GetByPaymentCodeAsync(ApplyPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[ApplyPaymentWebhookCommandHandler][GetByPaymentCodeAsync] Get By Payment Code {request.PaymentCode}");
            return await _saleRepository.GetByPaymentCodeAsync(request.PaymentCode, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApplyPaymentWebhookCommandHandler][GetByPaymentCodeAsync] Error on execute {ex.Message}");
            throw;
        }
    }

}
