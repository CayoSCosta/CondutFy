using CondutFy.Application.Common.Interfaces;
using CondutFy.Application.Webhooks.Commands.ReceiveMessage;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public WebhooksController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("{channelType}")]
    public async Task<IActionResult> Receive(string channelType, [FromBody] ReceiveMessageCommand command, CancellationToken cancellationToken)
    {
        var handler = new ReceiveMessageCommandHandler(_context);
        var success = await handler.HandleAsync(channelType, command, cancellationToken);

        if (!success)
        {
            return NotFound(new { error = "Channel not found, inactive, or invalid tenant mapping." });
        }

        return Ok(new { status = "Message processed successfully" });
    }
}