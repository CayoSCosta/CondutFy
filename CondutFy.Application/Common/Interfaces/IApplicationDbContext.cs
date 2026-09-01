using CondutFy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<ChannelIntegration> ChannelIntegrations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}