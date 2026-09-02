namespace CondutFy.Application.Common.Interfaces;

public interface IMessagingService
{
    Task<bool> SendTextMessageAsync(string channelType, string identifier, string accessToken, string recipientPhone, string messageContent, CancellationToken cancellationToken);
}