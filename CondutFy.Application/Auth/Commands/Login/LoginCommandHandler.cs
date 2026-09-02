using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CondutFy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CondutFy.Application.Auth.Commands.Login;

public class LoginCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string?> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        // 1. Busca o usuário pelo e-mail
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user == null)
        {
            return null; // Credenciais inválidas
        }

        // 2. Validação simples de senha (em produção real, use BCrypt ou PasswordHasher)
        if (user.PasswordHash != command.Password)
        {
            return null;
        }

        // 3. Gera o Token JWT contendo o TenantId nas Claims
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? "ChaveSecretaSuperSeguraDoCondutFyParaJWT2026!");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("tenantId", user.TenantId.ToString()) // 🔒 O segredo do multi-tenancy seguro!
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}