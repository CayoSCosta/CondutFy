using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Messages.Commands.ReceiveIncomingMessage;

public class ReceiveIncomingMessageCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IMessagingService _messagingService;
    private readonly IMessageSuggestorService _messageSuggestorService;

    public ReceiveIncomingMessageCommandHandler(
        IApplicationDbContext context, 
        IMessagingService messagingService, 
        IMessageSuggestorService messageSuggestorService)
    {
        _context = context;
        _messagingService = messagingService;
        _messageSuggestorService = messageSuggestorService;
    }

    public async Task<string> HandleAsync(ReceiveIncomingMessageCommand command, CancellationToken cancellationToken)
    {
        // 1. Salva a mensagem recebida do cliente no banco 
        var incomingMessage = new Message(
            Guid.NewGuid(),
            command.TenantId,
            command.SenderPhone,
            string.Empty,
            command.Content,
            isFromBot: false
        );
        _context.Messages.Add(incomingMessage);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Busca o canal integrado e ativo desse Tenant
        var channel = await _context.ChannelIntegrations
            .FirstOrDefaultAsync(c => c.TenantId == command.TenantId && c.IsConnected, cancellationToken);

        if (channel == null)
        {
            throw new InvalidOperationException("Nenhum canal ativo encontrado para este Tenant.");
        }

        // 3. Contexto do infoproduto (pode vir de uma config do tenant ou de uma string padrão por enquanto)
        string productContext = "Infoproduto digital de alta conversão comercializado via CondutFy.";

        // 4. Consulta a Inteligência Artificial passando a mensagem, o contexto e o token
        var aiResponse = await _messageSuggestorService.GenerateSmartResponseAsync(
            command.Content, 
            productContext, 
            cancellationToken
        );

        if (string.IsNullOrEmpty(aiResponse))
        {
            aiResponse = "Olá! Recebi sua mensagem e em breve um atendente humano irá te ajudar.";
        }

        // 5. Salva a resposta do Bot/IA no banco de histórico
        var botMessage = new Message(
            Guid.NewGuid(),
            command.TenantId,
            command.SenderPhone,
            "CondutFy Bot",
            aiResponse,
            isFromBot: true
        );
        _context.Messages.Add(botMessage);
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Dispara a resposta de volta para o cliente
        await _messagingService.SendTextMessageAsync(
            channel.ChannelType,
            channel.Identifier,
            channel.AccessToken,
            command.SenderPhone,
            aiResponse,
            cancellationToken
        );

        return aiResponse;
    }
}