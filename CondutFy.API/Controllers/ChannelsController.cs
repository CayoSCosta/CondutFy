using CondutFy.Application.Channels.Commands.CreateChannel;
using CondutFy.Application.Channels.Queries.GetChannelsByTenant;
using CondutFy.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ChannelsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChannelCommand command, CancellationToken cancellationToken)
    {
        var handler = new CreateChannelCommandHandler(_context);
        var channelId = await handler.HandleAsync(command, cancellationToken);

        return CreatedAtAction(nameof(GetByTenant), new { tenantId = command.TenantId }, new { Id = channelId });
    }

    [HttpGet("tenant/{tenantId:guid}")]
    public async Task<IActionResult> GetByTenant(Guid tenantId, CancellationToken cancellationToken)
    {
        var handler = new GetChannelsByTenantQueryHandler(_context);
        var channels = await handler.HandleAsync(tenantId, cancellationToken);

        return Ok(channels);
    }
}