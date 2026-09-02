using CondutFy.Application.Common.Interfaces;
using CondutFy.Application.Webhooks.Commands.ProcessBillingWebhook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Garantir o using

namespace CondutFy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IMessagingService _messagingService;
    private readonly ILogger<ProcessBillingWebhookCommandHandler> _handlerLogger;

    public BillingController(
        IApplicationDbContext context, 
        IMessagingService messagingService, 
        ILogger<ProcessBillingWebhookCommandHandler> handlerLogger)
    {
        _context = context;
        _messagingService = messagingService;
        _handlerLogger = handlerLogger;
    }

    [HttpPost("webhook/{provider}")]
    public async Task<IActionResult> ReceiveWebhook(string provider, [FromBody] ProcessBillingWebhookCommand command, CancellationToken cancellationToken)
    {
        var normalizedCommand = command with { Provider = provider.ToLower() };

        var handler = new ProcessBillingWebhookCommandHandler(_context, _messagingService, _handlerLogger);
        await handler.HandleAsync(normalizedCommand, cancellationToken);

        return Ok(new { status = $"Webhook from {provider} received and processed successfully" });
    }
}