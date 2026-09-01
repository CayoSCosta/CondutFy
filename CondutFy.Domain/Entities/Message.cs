namespace CondutFy.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ChannelIntegrationId { get; private set; }
    public string SenderPhone { get; private set; }
    public string Content { get; private set; }
    public string ExternalMessageId { get; private set; }
    public bool IsFromBot { get; private set; } // true = Enviado por nós, false = Recebido do cliente
    public DateTime CreatedAt { get; private set; }

    // Construtor vazio para o EF Core
    protected Message() { }

    public Message(Guid tenantId, Guid channelIntegrationId, string senderPhone, string content, string externalMessageId, bool isFromBot = false)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ChannelIntegrationId = channelIntegrationId;
        SenderPhone = senderPhone;
        Content = content;
        ExternalMessageId = externalMessageId;
        IsFromBot = isFromBot;
        CreatedAt = DateTime.UtcNow;
    }
}