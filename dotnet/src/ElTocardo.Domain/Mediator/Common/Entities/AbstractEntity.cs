namespace ElTocardo.Domain.Mediator.Common.Entities;

public interface IEntity<out TId, out TKey>
{
    public TId Id { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }
    public TKey GetKey();
}

public abstract class AbstractEntity<TId,TKey> : IEntity<TId,TKey>
{
    public  abstract TId Id { get; }
    public  DateTimeOffset CreatedAt { get; protected init; }
    public  DateTimeOffset UpdatedAt { get; protected set; }

    public abstract TKey GetKey();
}
