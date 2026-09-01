using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Webhooks.Commands.ReceiveMessage;

public class ReceiveMessageCommandHandler
{
    private readonly IApplicationDbContext _context;

    public ReceiveMessageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(string channelType, ReceiveMessageCommand command, CancellationToken cancellationToken)
    {
        var channelIntegration = await _context.ChannelIntegrations
            .FirstOrDefaultAsync(c => c.ChannelType == channelType && c.Identifier == command.Identifier, cancellationToken);

        if (channelIntegration == null || !channelIntegration.IsConnected)
        {
            return false;
        }

        // Criamos a entidade Message amarrada ao Tenant e ao Canal
        var message = new Message(
            tenantId: channelIntegration.TenantId,
            channelIntegrationId: channelIntegration.Id,
            senderPhone: command.SenderPhone,
            content: command.MessageContent,
            externalMessageId: command.ExternalMessageId,
            isFromBot: false // É uma mensagem entrando, então não é do bot
        );

        // Salvamos no banco
        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"[WEBHOOK SALVO NO BANCO] Tenant: {channelIntegration.TenantId} | De: {command.SenderPhone}");

        return true;
    }
}