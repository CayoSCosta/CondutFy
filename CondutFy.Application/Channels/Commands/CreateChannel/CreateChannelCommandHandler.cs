using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities;

namespace CondutFy.Application.Channels.Commands.CreateChannel;

public class CreateChannelCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService; // 👈 1. Injetamos o serviço de usuário atual

    public CreateChannelCommandHandler(
        IApplicationDbContext context, 
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> HandleAsync(CreateChannelCommand command, CancellationToken cancellationToken)
    {
        // 🔒 Pega o TenantId direto das claims do Token JWT com segurança total
        var tenantId = _currentUserService.TenantId;

        if (tenantId == null)
        {
            throw new UnauthorizedAccessException("Usuário não autenticado ou Tenant não identificado.");
        }

        // Cria a integração amarrada estritamente ao Tenant do usuário logado
        var channel = new ChannelIntegration(
            tenantId.Value,
            command.ChannelType,
            command.Identifier,
            command.AccessToken
        );

        _context.ChannelIntegrations.Add(channel);
        await _context.SaveChangesAsync(cancellationToken);

        return channel.Id;
    }
}