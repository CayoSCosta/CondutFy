using CondutFy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Dashboard.Queries.GetDashboardMetrics;

public class DashboardMetricsDto
{
    public int TotalMessages { get; set; }
    public int ConnectedChannelsCount { get; set; }
    public string TenantName { get; set; } = string.Empty;
}

public class GetDashboardMetricsQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDashboardMetricsQueryHandler(
        IApplicationDbContext context, 
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardMetricsDto> HandleAsync(CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        if (tenantId == null)
        {
            throw new UnauthorizedAccessException("Usuário não autenticado ou Tenant não identificado.");
        }

        // Busca o nome do Tenant
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId.Value, cancellationToken);

        // Conta as mensagens do tenant
        var totalMessages = await _context.Messages
            .Where(m => m.TenantId == tenantId.Value)
            .CountAsync(cancellationToken);

        // Conta os canais conectados do tenant
        var connectedChannels = await _context.ChannelIntegrations
            .Where(c => c.TenantId == tenantId.Value && c.IsConnected)
            .CountAsync(cancellationToken);

        return new DashboardMetricsDto
        {
            TenantName = tenant?.Name ?? "Desconhecido",
            TotalMessages = totalMessages,
            ConnectedChannelsCount = connectedChannels
        };
    }
}