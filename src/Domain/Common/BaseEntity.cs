namespace CleanArchitecture.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; protected set; }

    public bool Active { get; protected set; } = true;

    protected void Touch() => UpdatedAt = DateTime.UtcNow;

    public void Deactivate()
    {
        Active = false;
        Touch();
    }

    public void Reactivate()
    {
        Active = true;
        Touch();
    }
}
