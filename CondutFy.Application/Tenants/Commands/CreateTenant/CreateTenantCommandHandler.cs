using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities;

namespace CondutFy.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler
{
    private readonly IApplicationDbContext _context; // 👈 Usa a interface

    public CreateTenantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> HandleAsync(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = new Tenant(command.Name, command.Document);

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }
}