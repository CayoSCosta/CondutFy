using CondutFy.Application.Common.Interfaces;
using CondutFy.Application.Messages.Commands.SendMessage;
using CondutFy.Application.Messages.Queries.GetConversationHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy. API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IMessagingService _messagingService;
    private readonly ICurrentUserService _currentUserService;

    public MessagesController(
        IApplicationDbContext context,
        IMessagingService messagingService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _messagingService = messagingService;
        _currentUserService = currentUserService;
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

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] string? senderPhone, CancellationToken cancellationToken)
    {
        var query = new GetConversationHistoryQuery(senderPhone);
        var handler = new GetConversationHistoryQueryHandler(_context, _currentUserService);

        var history = await handler.HandleAsync(query, cancellationToken);

        return Ok(history);
    }
}