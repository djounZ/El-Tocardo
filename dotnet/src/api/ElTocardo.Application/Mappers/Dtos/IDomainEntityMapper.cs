namespace ElTocardo.Application.Mappers.Dtos;

public interface IDomainEntityMapper<TDomainEntity, TApplicationEntity>
{
    public TApplicationEntity ToApplication(TDomainEntity domainItem);

    public TDomainEntity ToDomain(TApplicationEntity applicationItem);
}

public static class DomainEntityMapperExtension
{
    public static TApplicationEntity? ToApplicationNullable<TDomainEntity, TApplicationEntity>(
        this IDomainEntityMapper<TDomainEntity, TApplicationEntity> mapper,
        TDomainEntity? item)
        where TDomainEntity : class
        where TApplicationEntity : class
    {
        return item == null ? null : mapper.ToApplication(item);
    }
    public static TDomainEntity? ToDomainNullable<TDomainEntity, TApplicationEntity>(
        this IDomainEntityMapper<TDomainEntity, TApplicationEntity> mapper,
        TApplicationEntity? item)
        where TDomainEntity : class
        where TApplicationEntity : class
    {
        return item == null ? null : mapper.ToDomain(item);
    }

    public static TApplicationEntity? ToApplication<TDomainEntity, TApplicationEntity>(
        this IDomainEntityMapper<TDomainEntity, TApplicationEntity> mapper,
        TDomainEntity? item)
    where TDomainEntity : struct
    where TApplicationEntity : struct
    {
        return item == null ? null : mapper.ToApplication(item.Value);
    }
    public static TDomainEntity? ToDomain<TDomainEntity, TApplicationEntity>(
        this IDomainEntityMapper<TDomainEntity, TApplicationEntity> mapper,
        TApplicationEntity? item)
        where TDomainEntity : struct
        where TApplicationEntity : struct
    {
        return item == null ? null : mapper.ToDomain(item.Value);
    }

}
