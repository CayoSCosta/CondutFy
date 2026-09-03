namespace CondutFy.Application.Messages.Queries.GetConversationHistory;

public record GetConversationHistoryQuery(
    //Guid TenantId,
    string? SenderPhone = null // Opcional: filtrar por telefone do cliente específico
);