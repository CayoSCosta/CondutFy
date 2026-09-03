using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities; // 👈 Importante para usar Tenant e User
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
            return true; 
        }

        _logger.LogInformation("[BILLING] Pagamento aprovado via {Provider} | Cliente: {Name} ({Phone}) | Produto: {Product}", 
            command.Provider, command.CustomerName, command.CustomerPhone, command.ProductName);

        // 2. Provisionamento Automático de Conta (Multi-tenancy por Design)
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == command.CustomerEmail, cancellationToken);

        if (existingUser == null)
        {
            // Se o cliente ainda não tem conta, criamos um Tenant e um Usuário para ele
            string tenantName = $"Infoproduto de {command.CustomerName}";
            string defaultDocument = "00.000.000/0001-00"; // Ou extraído do payload se a Kiwify enviar CPF/CNPJ
            string temporaryPassword = "ChangeMe123!"; // Senha provisória inicial

            var tenant = new Tenant(tenantName, defaultDocument);
            _context.Tenants.Add(tenant);

            var user = new User(tenant.Id, command.CustomerName, command.CustomerEmail, temporaryPassword);
            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[BILLING] Novo Tenant ({TenantId}) e Usuário ({Email}) criados automaticamente via webhook.", tenant.Id, command.CustomerEmail);
        }
        else
        {
            _logger.LogInformation("[BILLING] Cliente {Email} já possui conta ativa no sistema. Acesso liberado ao produto.", command.CustomerEmail);
        }

        // 3. Disparar a mensagem de boas-vindas / entrega automática no WhatsApp do cliente
        var defaultChannel = await _context.ChannelIntegrations
            .FirstOrDefaultAsync(c => c.IsConnected, cancellationToken);

        if (defaultChannel != null && !string.IsNullOrEmpty(command.CustomerPhone))
        {
            string welcomeMessage = $"Olá {command.CustomerName}! Sua compra do produto '{command.ProductName}' foi aprovada com sucesso. Seu acesso ao CondutFy foi liberado!";

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