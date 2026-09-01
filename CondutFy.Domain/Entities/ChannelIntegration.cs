namespace CondutFy.Domain.Entities;

public class ChannelIntegration
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; } // Chave que garante o multitenancy por coluna
    public string ChannelType { get; private set; } // Ex: "WhatsApp", "Instagram", "Telegram"
    public string Identifier { get; private set; } // Número, ID da página, etc.
    public string AccessToken { get; private set; } // Token da API oficial/provedor
    public bool IsConnected { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected ChannelIntegration() { }

    public ChannelIntegration(Guid tenantId, string channelType, string identifier, string accessToken)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ChannelType = channelType;
        Identifier = identifier;
        AccessToken = accessToken;
        IsConnected = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateToken(string newToken) => AccessToken = newToken;
    public void Disconnect() => IsConnected = false;
}