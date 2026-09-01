using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities;

namespace CondutFy.Application.Channels.Commands.CreateChannel;

public class CreateChannelCommandHandler
{
    private readonly IApplicationDbContext _context;

    public CreateChannelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> HandleAsync(CreateChannelCommand command, CancellationToken cancellationToken)
    {
        // Cria a integração amarrada ao TenantId (garantindo o multi-tenant)
        var channel = new ChannelIntegration(
            command.TenantId,
            command.ChannelType,
            command.Identifier,
            command.AccessToken
        );

        _context.ChannelIntegrations.Add(channel);
        await _context.SaveChangesAsync(cancellationToken);

        return channel.Id;
    }
}