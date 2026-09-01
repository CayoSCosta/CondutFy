namespace CondutFy.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Document { get; private set; } // CNPJ ou CPF
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Construtor protegido para o EF Core / DDD
    protected Tenant() { }

    public Tenant(string name, string document)
    {
        Id = Guid.NewGuid();
        Name = GuardAgainstInvalidName(name);
        Document = document;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    private static string GuardAgainstInvalidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do Tenant não pode ser vazio.", nameof(name));
        return name;
    }
}