namespace CondutFy.Application.Common.Interfaces;

public interface IMessageSuggestorService
{
    Task<string> GenerateSmartResponseAsync(string customerMessage, string productContext, CancellationToken cancellationToken);
}