namespace ElTocardo.Application.Mediator.Common.Queries;

public abstract record GetByKeyQueryBase<TKey>(TKey Key);
public abstract record GetByIdQueryBase<TKey>(TKey Id);
