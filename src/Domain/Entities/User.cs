using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.Entities;

public class User : BaseEntity
{
    private User()
    {
    }

    public User(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("User name is required.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new DomainException("A valid user email is required.");
        }

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
    }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;
}
