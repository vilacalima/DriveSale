using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Client : EntityBase, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Cpf { get; private set; } = default!;

    private Client() { }

    public Client(string name, string email, Cpf cpf)
    {
        Update(name, email);
        Cpf = cpf.Value;
    }

    public void Update(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome é obrigatório", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email é obrigatório", nameof(email));
        Name = name.Trim();
        Email = email.Trim();
        Touch();
    }
}

