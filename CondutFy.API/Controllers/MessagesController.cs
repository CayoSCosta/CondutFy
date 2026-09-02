using CondutFy.Application.Common.Interfaces;
using CondutFy.Application.Messages.Commands.SendMessage;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IMessagingService _messagingService;

    public MessagesController(IApplicationDbContext context, IMessagingService messagingService)
    {
        _context = context;
        _messagingService = messagingService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendMessageCommand command, CancellationToken cancellationToken)
    {
        var handler = new SendMessageCommandHandler(_context, _messagingService);
        var success = await handler.HandleAsync(command, cancellationToken);

        if (!success)
        {
            return BadRequest(new { error = "Failed to send message. Check tenant, channel integration, or connection status." });
        }

        return Ok(new { status = "Message sent and logged successfully" });
    }
}