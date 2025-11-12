namespace Mindly.Domain.Entities;

public abstract class BaseEntity
{
    protected BaseEntity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("O identificador não pode ser vazio.", nameof(id));
        }

        Id = id;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; protected set; }
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }

    public void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

