namespace CondutFy.Application.Messages.Commands.ReceiveIncomingMessage;

public record ReceiveIncomingMessageCommand(
    Guid TenantId,
    string SenderPhone,
    string Content
);