using CondutFy.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CondutFy.Infrastructure.Services;

public class ExternalMessagingService : IMessagingService
{
    private readonly ILogger<ExternalMessagingService> _logger;

    public ExternalMessagingService(ILogger<ExternalMessagingService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendTextMessageAsync(string channelType, string identifier, string accessToken, string recipientPhone, string messageContent, CancellationToken cancellationToken)
    {
        // 🚀 AQUI ENTRARIA O RESTCLIENT / HTTPCLIENT PARA A META, EVOLUTION API, Z-API, ETC.
        // Exemplo simulado de envio com sucesso:
        _logger.LogInformation("[ENVIO DE MENSAGEM] Canal: {Channel} | Instância/ID: {Identifier} | Para: {Phone} | Conteúdo: {Content}", 
            channelType, identifier, recipientPhone, messageContent);

        // Simulando delay de rede da API externa
        await Task.Delay(100, cancellationToken);

        return true; // Sucesso no envio
    }
}