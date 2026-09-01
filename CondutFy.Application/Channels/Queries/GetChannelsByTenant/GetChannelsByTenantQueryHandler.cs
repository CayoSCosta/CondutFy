using CondutFy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Channels.Queries.GetChannelsByTenant;

public class GetChannelsByTenantQueryHandler
{
    private readonly IApplicationDbContext _context;

    public GetChannelsByTenantQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChannelDto>> HandleAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        // Graças ao Global Query Filter do EF Core, ele já filtra sozinho caso o tenant venha injetado, 
        // mas aqui fazemos o filtro explícito por TenantId.
        return await _context.ChannelIntegrations
            .Where(c => c.TenantId == tenantId)
            .Select(c => new ChannelDto
            {
                Id = c.Id,
                TenantId = c.TenantId,
                ChannelType = c.ChannelType,
                Identifier = c.Identifier,
                IsConnected = c.IsConnected,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}