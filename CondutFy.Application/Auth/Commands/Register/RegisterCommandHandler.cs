using CondutFy.Application.Common.Interfaces;
using CondutFy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Auth.Commands.Register;

public class RegisterCommandHandler
{
    private readonly IApplicationDbContext _context;

    public RegisterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        // Verifica se o e-mail já existe
        var userExists = await _context.Users
            .AnyAsync(u => u.Email == command.Email, cancellationToken);

        if (userExists)
        {
            return false;
        }

        // 1. Cria o Tenant passando o nome corretamente conforme o construtor da entidade
        var tenant = new Tenant(command.TenantName, command.Document); 
        _context.Tenants.Add(tenant);

        // 2. Cria o Usuário Admin vinculado ao Tenant recém-criado (usando o ID gerado pelo Tenant)
        var user = new User(tenant.Id, command.Name, command.Email, command.Password);
        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}