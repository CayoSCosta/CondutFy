using CondutFy.Application.Common.Interfaces;
using CondutFy.Application.Messages.Commands.ReceiveIncomingMessage;
using CondutFy.Application.Messages.Commands.SendMessage;
using CondutFy.Application.Messages.Queries.GetConversationHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IMessagingService _messagingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessageSuggestorService _messageSuggestorService;

    public MessagesController(
        IApplicationDbContext context,
        IMessagingService messagingService,
        ICurrentUserService currentUserService,
        IMessageSuggestorService messageSuggestorService)
    {
        _context = context;
        _messagingService = messagingService;
        _currentUserService = currentUserService;
        _messageSuggestorService = messageSuggestorService;
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
    [HttpPost("incoming")]
    public async Task<IActionResult> ReceiveIncoming([FromBody] ReceiveIncomingMessageCommand command, CancellationToken cancellationToken)
    {
        // O handler recebe o context e o messagingService. 
        // Se o seu handler também precisar do suggestor service, injetamos ele no controller e passamos para o handler.
        var handler = new ReceiveIncomingMessageCommandHandler(_context, _messagingService, _messageSuggestorService);
        var reply = await handler.HandleAsync(command, cancellationToken);

        return Ok(new { success = true, reply });
    }
}