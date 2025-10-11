namespace Application.Payments.DTOs;

public record PaymentWebhookDto(string Status, string? Provider = null, string? EventId = null);

