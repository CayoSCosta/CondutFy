using CondutFy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Tenants.Queries.GetTenants;

public class GetTenantsQueryHandler
{
    private readonly IApplicationDbContext _context;

    public GetTenantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TenantDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return await _context.Tenants
            .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Document = t.Document,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}