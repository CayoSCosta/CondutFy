namespace CondutFy.Application.Messages.Commands.SendMessage;

public record SendMessageCommand(
    Guid TenantId,
    Guid ChannelIntegrationId,
    string RecipientPhone,
    string Content
);