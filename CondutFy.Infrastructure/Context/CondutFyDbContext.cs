using CondutFy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Infrastructure.Context;

public class CondutFyDbContext : DbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ChannelIntegration> ChannelIntegrations => Set<ChannelIntegration>();

    private readonly Guid? _currentTenantId; // Aqui guardaremos o Tenant atual da requisição

    public CondutFyDbContext(DbContextOptions<CondutFyDbContext> options) : base(options)
    {
    }

    // Construtor auxiliar caso precise injetar o tenant atual futuramente
    public CondutFyDbContext(DbContextOptions<CondutFyDbContext> options, Guid? currentTenantId) : base(options)
    {
        _currentTenantId = currentTenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica o mapeamento de todas as entidades do assembly atual
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CondutFyDbContext).Assembly);

        // 🔒 O GLOBAL QUERY FILTER PARA MULTITENANCY POR COLUNA
        // Se a entidade tiver TenantId, o EF Core filtra automaticamente nos SELECTs
        modelBuilder.Entity<ChannelIntegration>().HasQueryFilter(c => !_currentTenantId.HasValue || c.TenantId == _currentTenantId.Value);
    }
}