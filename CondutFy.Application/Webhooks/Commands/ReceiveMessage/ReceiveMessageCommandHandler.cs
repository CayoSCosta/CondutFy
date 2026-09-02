using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Webhooks.Commands.ReceiveMessage;

public class ReceiveMessageCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IMessageSuggestorService _aiService;
    private readonly IMessagingService _messagingService;

    public ReceiveMessageCommandHandler(
        IApplicationDbContext context, 
        IMessageSuggestorService aiService, 
        IMessagingService messagingService)
    {
        _context = context;
        _aiService = aiService;
        _messagingService = messagingService;
    }

    public async Task<bool> HandleAsync(string channelType, ReceiveMessageCommand command, CancellationToken cancellationToken)
    {
        var channelIntegration = await _context.ChannelIntegrations
            .FirstOrDefaultAsync(c => c.ChannelType == channelType && c.Identifier == command.Identifier, cancellationToken);

        if (channelIntegration == null || !channelIntegration.IsConnected)
        {
            return false;
        }

        // 1. Salva a mensagem recebida do cliente no banco
        var message = new Message(
            tenantId: channelIntegration.TenantId,
            channelIntegrationId: channelIntegration.Id,
            senderPhone: command.SenderPhone,
            content: command.MessageContent,
            externalMessageId: command.ExternalMessageId,
            isFromBot: false
        );

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. 🤖 Gera a resposta inteligente com a IA
        string smartResponse = await _aiService.GenerateSmartResponseAsync(
            command.MessageContent, 
            productContext: "SaaS de Automação Omnichannel e Infoprodutos", 
            cancellationToken
        );

        // 3. 🚀 Dispara a resposta de volta para o cliente
        await _messagingService.SendTextMessageAsync(
            channelIntegration.ChannelType,
            channelIntegration.Identifier,
            channelIntegration.AccessToken,
            command.SenderPhone,
            smartResponse,
            cancellationToken
        );

        return true;
    }
}