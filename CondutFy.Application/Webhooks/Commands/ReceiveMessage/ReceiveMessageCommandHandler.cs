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

    public async Task<bool> HandleAsync(string channelType, ReceiveMessageCommand command, CancellationToken cancellationToken)
    {
        var channelIntegration = await _context.ChannelIntegrations
            .FirstOrDefaultAsync(c => c.ChannelType == channelType && c.Identifier == command.Identifier, cancellationToken);

        if (channelIntegration == null || !channelIntegration.IsConnected)
        {
            return false;
        }

        var tenantId = channelIntegration.TenantId;

        Console.WriteLine($"[WEBHOOK RECEBIDO] Tenant: {tenantId} | Canal: {channelType} | De: {command.SenderPhone} | Msg: {command.MessageContent}");

        return true;
    }
}