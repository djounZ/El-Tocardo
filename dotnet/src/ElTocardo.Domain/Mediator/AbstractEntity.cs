namespace ElTocardo.Domain.Mediator;

public abstract class AbstractEntity<TKey>
{
    public  Guid Id { get; protected init; }
    public  DateTime CreatedAt { get; protected init; }
    public  DateTime UpdatedAt { get; protected set; }

    public abstract TKey GetKey();
}
