namespace Mecanica.Hermes.Domain.Common.Abstractions;

public abstract class BaseDomain
{
    public Guid Id { get; internal set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected internal void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    protected internal void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    protected internal void RestaurarBase(Guid id, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsDeleted = isDeleted;
    }
}
