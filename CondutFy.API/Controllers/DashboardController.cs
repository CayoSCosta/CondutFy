using CondutFy.Application.Common.Interfaces;
using CondutFy.Application.Dashboard.Queries.GetDashboardMetrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DashboardController(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics(CancellationToken cancellationToken)
    {
        var handler = new GetDashboardMetricsQueryHandler(_context, _currentUserService);
        var metrics = await handler.HandleAsync(cancellationToken);

        return Ok(metrics);
    }
}