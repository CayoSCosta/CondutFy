using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Messages.Commands.SendMessage;

public class SendMessageCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IMessagingService _messagingService;

    public SendMessageCommandHandler(IApplicationDbContext context, IMessagingService messagingService)
    {
        _context = context;
        _messagingService = messagingService;
    }

    public async Task<bool> HandleAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        // 1. Buscar a integração do canal para garantir que ela pertence ao Tenant e pegar o AccessToken/Identificador
        var channel = await _context.ChannelIntegrations
            .FirstOrDefaultAsync(c => c.Id == command.ChannelIntegrationId && c.TenantId == command.TenantId, cancellationToken);

        if (channel == null || !channel.IsConnected)
        {
            return false; // Canal não encontrado ou inativo
        }

        // 2. Disparar o envio real através do serviço de mensageria externo
        var sentSuccessfully = await _messagingService.SendTextMessageAsync(
            channel.ChannelType,
            channel.Identifier,
            channel.AccessToken,
            command.RecipientPhone,
            command.Content,
            cancellationToken
        );

        if (!sentSuccessfully)
        {
            return false;
        }

        // 3. Salvar a mensagem no banco de dados como sendo enviada pelo Bot/Atendente (isFromBot = true)
        var message = new Message(
            tenantId: command.TenantId,
            channelIntegrationId: command.ChannelIntegrationId,
            senderPhone: command.RecipientPhone,
            content: command.Content,
            externalMessageId: $"sent_{Guid.NewGuid()}",
            isFromBot: true // 👈 Marcada como enviada por nós
        );

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}