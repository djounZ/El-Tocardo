namespace ElTocardo.Domain.Mediator.Queries;

public abstract record GetByKeyQueryBase<TKey>(TKey Key);
public abstract record GetByIdQueryBase<TKey>(TKey Id);
