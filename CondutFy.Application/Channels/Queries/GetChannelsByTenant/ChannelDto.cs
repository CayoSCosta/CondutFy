namespace CondutFy.Application.Channels.Queries.GetChannelsByTenant;

public class ChannelDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ChannelType { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime CreatedAt { get; set; }
}