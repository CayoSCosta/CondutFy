namespace CondutFy.Application.Webhooks.Commands.ProcessBillingWebhook;

public record ProcessBillingWebhookCommand(
    string Provider,        // Ex: "kiwify", "hotmart", "stripe"
    string EventType,       // Ex: "approved", "refunded", "cancelled"
    string ExternalSaleId,  // ID único da transação na plataforma
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string ProductName
);  