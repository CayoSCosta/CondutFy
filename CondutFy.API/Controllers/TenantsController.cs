using CondutFy.Application.Common.Interfaces;
using CondutFy.Application.Tenants.Commands.CreateTenant;
using CondutFy.Application.Tenants.Queries.GetTenants; // 👈 Importa a query
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public TenantsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var handler = new CreateTenantCommandHandler(_context);
        var tenantId = await handler.HandleAsync(command, cancellationToken);

        return CreatedAtAction(nameof(Create), new { id = tenantId }, new { Id = tenantId });
    }

    // 🔍 NOVO ENDPOINT DE CONSULTA
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var handler = new GetTenantsQueryHandler(_context);
        var tenants = await handler.HandleAsync(cancellationToken);

        return Ok(tenants);
    }
}