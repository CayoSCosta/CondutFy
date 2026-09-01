namespace CondutFy.Application.Channels.Commands.CreateChannel;

public record CreateChannelCommand(
    Guid TenantId,
    string ChannelType, // Ex: "WhatsApp", "Telegram", "Instagram"
    string Identifier,  // Ex: Número do WhatsApp ou ID da página
    string AccessToken  // Token oficial da API do provedor
);