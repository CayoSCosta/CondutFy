using CondutFy.Application.Auth.Commands.Login;
using CondutFy.Application.Auth.Commands.Register;
using CondutFy.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CondutFy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var handler = new RegisterCommandHandler(_context);
        var success = await handler.HandleAsync(command, cancellationToken);

        if (!success)
        {
            return BadRequest(new { error = "E-mail já cadastrado no sistema." });
        }

        return Ok(new { message = "Conta e Tenant criados com sucesso!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var handler = new LoginCommandHandler(_context, _configuration);
        var token = await handler.HandleAsync(command, cancellationToken);

        if (token == null)
        {
            return Unauthorized(new { error = "E-mail ou senha inválidos." });
        }

        return Ok(new { accessToken = token, tokenType = "Bearer" });
    }
}