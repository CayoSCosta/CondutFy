using CondutFy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Webhooks.Commands.ReceiveMessage;

public class ReceiveMessageCommandHandler
{
    private readonly IApplicationDbContext _context;

    public ReceiveMessageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(ReceiveMessageCommand command, CancellationToken cancellationToken)
    {
        // 1. Validar se existe uma integração cadastrada para esse canal e identificador
        var channelIntegration = await _context.ChannelIntegrations
            .FirstOrDefaultAsync(c => c.ChannelType == command.ChannelType && c.Identifier == command.Identifier, cancellationToken);

        if (channelIntegration == null || !channelIntegration.IsConnected)
        {
            // Canal não encontrado ou inativo, rejeitamos o webhook
            return false;
        }

        // 2. Aqui o TenantId é obtido direto da integração validada! (Isolamento perfeito)
        var tenantId = channelIntegration.TenantId;

        // TODO: Futuramente, vamos salvar a mensagem no banco de dados e disparar o motor de IA/Atendimento.
        // Por enquanto, vamos registrar no console para vermos o webhook funcionando em tempo real.
        Console.WriteLine($"[WEBHOOK RECEBIDO] Tenant: {tenantId} | Canal: {command.ChannelType} | De: {command.SenderPhone} | Msg: {command.MessageContent}");

        return true;
    }
}