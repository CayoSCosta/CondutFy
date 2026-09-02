namespace CondutFy.Application.Auth.Commands.Register;

public record RegisterCommand(
    string TenantName,
    string Document,
    string Name,
    string Email,
    string Password
);