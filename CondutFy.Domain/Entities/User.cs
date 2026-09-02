namespace CondutFy.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; } // 🔑 Chave de ouro para o multi-tenancy
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public User(Guid tenantId, string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }
}