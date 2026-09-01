using CondutFy.Application.Common.Interfaces; // 👈 Importa o contrato da aplicação
using CondutFy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Infrastructure.Context;

public class CondutFyDbContext : DbContext, IApplicationDbContext // 👈 Assina a interface aqui
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ChannelIntegration> ChannelIntegrations => Set<ChannelIntegration>();

    private readonly Guid? _currentTenantId;

    public CondutFyDbContext(DbContextOptions<CondutFyDbContext> options) : base(options)
    {
    }

    public CondutFyDbContext(DbContextOptions<CondutFyDbContext> options, Guid? currentTenantId) : base(options)
    {
        _currentTenantId = currentTenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CondutFyDbContext).Assembly);

        // 👇 Ajuste para evitar o erro de Nullable no Global Query Filter
        modelBuilder.Entity<ChannelIntegration>().HasQueryFilter(c => _currentTenantId == null || c.TenantId == _currentTenantId);
    }
}