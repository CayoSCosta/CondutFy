using CondutFy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CondutFy.Application.Webhooks.Commands.ProcessBillingWebhook;

public class ProcessBillingWebhookCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IMessagingService _messagingService;
    private readonly ILogger<ProcessBillingWebhookCommandHandler> _logger;

    public ProcessBillingWebhookCommandHandler(
        IApplicationDbContext context, 
        IMessagingService messagingService,
        ILogger<ProcessBillingWebhookCommandHandler> logger)
    {
        _context = context;
        _messagingService = messagingService;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(ProcessBillingWebhookCommand command, CancellationToken cancellationToken)
    {
        // 1. Validamos se o evento é de compra aprovada
        if (!command.EventType.Equals("approved", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("[BILLING] Evento ignorado ({Event}) para o cliente {Email}", command.EventType, command.CustomerEmail);
            return true; // Retorna true para a plataforma não ficar reenviando o webhook de eventos irrelevantes
        }

        _logger.LogInformation("[BILLING] Pagamento aprovado via {Provider} | Cliente: {Name} ({Phone}) | Produto: {Product}", 
            command.Provider, command.CustomerName, command.CustomerPhone, command.ProductName);

        // TODO: Aqui você pode adicionar a lógica de provisionamento:
        // - Verificar se já existe um Tenant para o e-mail do cliente, senão criar automaticamente.
        // - Criar o vínculo de acesso ao produto.

        // 2. Disparar a mensagem de boas-vindas / entrega automática no WhatsApp do cliente
        // (Buscamos um canal ativo ou padrão do sistema para realizar o disparo)
        var defaultChannel = await _context.ChannelIntegrations
            .FirstOrDefaultAsync(c => c.IsConnected, cancellationToken);

        if (defaultChannel != null && !string.IsNullOrEmpty(command.CustomerPhone))
        {
            string welcomeMessage = $"Olá {command.CustomerName}! Sua compra do produto '{command.ProductName}' foi aprovada com sucesso. Seja bem-vindo!";

            await _messagingService.SendTextMessageAsync(
                defaultChannel.ChannelType,
                defaultChannel.Identifier,
                defaultChannel.AccessToken,
                command.CustomerPhone,
                welcomeMessage,
                cancellationToken
            );
        }

        return true;
    }
}