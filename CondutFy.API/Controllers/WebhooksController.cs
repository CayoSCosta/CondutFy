using CondutFy.Application.Common.Interfaces;
using CondutFy.Application.Webhooks.Commands.ReceiveMessage;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IMessageSuggestorService _aiService;
    private readonly IMessagingService _messagingService; // 👈 Já garantimos a injeção do envio também

    public WebhooksController(
        IApplicationDbContext context, 
        IMessageSuggestorService aiService, 
        IMessagingService messagingService)
    {
        _context = context;
        _aiService = aiService;
        _messagingService = messagingService;
    }

    [HttpPost("{channelType}")]
    public async Task<IActionResult> Receive(string channelType, [FromBody] ReceiveMessageCommand command, CancellationToken cancellationToken)
    {
        // Instancia o handler passando todos os serviços exigidos pelo construtor
        var handler = new ReceiveMessageCommandHandler(_context, _aiService, _messagingService);
        var success = await handler.HandleAsync(channelType, command, cancellationToken);

        if (!success)
        {
            return BadRequest(new { error = "Channel integration not found or disconnected." });
        }

        return Ok(new { status = "Message received, processed by AI, and sent successfully" });
    }
}