using CondutFy.Application.Channels.Commands.CreateChannel;
using CondutFy.Application.Channels.Queries.GetChannelsByTenant;
using CondutFy.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ChannelsController(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChannelCommand command, CancellationToken cancellationToken)
    {
        // 👈 Passamos tanto o context quanto o currentUserService para o handler
        var handler = new CreateChannelCommandHandler(_context, _currentUserService);
        var channelId = await handler.HandleAsync(command, cancellationToken);

        return Ok(new { id = channelId, message = "Canal integrado com sucesso!" });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyChannels(CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;
        if (tenantId == null)
        {
            return Unauthorized();
        }

        var handler = new GetChannelsByTenantQueryHandler(_context);
        var channels = await handler.HandleAsync(tenantId.Value, cancellationToken);

        return Ok(channels);
    }
}