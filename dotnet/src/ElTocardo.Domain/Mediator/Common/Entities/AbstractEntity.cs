namespace ElTocardo.Domain.Mediator.Common.Entities;

public interface IEntity<out TKey>
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    public TKey GetKey();
}

public abstract class AbstractEntity<TKey> : IEntity<TKey>
{
    public  Guid Id { get; protected init; }
    public  DateTime CreatedAt { get; protected init; }
    public  DateTime UpdatedAt { get; protected set; }

    public abstract TKey GetKey();
}
