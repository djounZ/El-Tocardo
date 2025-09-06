using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

public class UserExternalToken : AbstractEntity<Guid,UserExternalTokenKey>
{

    private UserExternalToken() { } // EF Core constructor

    public UserExternalToken(
        string userId,
        string provider,
        string value)
    {

        Id = Guid.NewGuid();
        UserId = userId;
        Provider = provider;
        Value = value;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateConfiguration();
    }

    public override Guid Id { get; }
    public string UserId { get; set; } = null!;
    public string Provider { get; set; } = null!;

    public string Value { get; set; } = null!;


    public void Update(
        string value)
    {
        Value = value;
        UpdatedAt = DateTime.UtcNow;

        ValidateConfiguration();
    }

    private void ValidateConfiguration()
    {

        if (string.IsNullOrWhiteSpace(UserId))
        {
            throw new ArgumentException("User Id cannot be null or empty", nameof(UserId));
        }

        if (string.IsNullOrWhiteSpace(Provider))
        {
            throw new ArgumentException("Provider cannot be null or empty", nameof(Value));
        }

        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new ArgumentException("Value cannot be null or empty", nameof(Value));
        }
    }

    public override UserExternalTokenKey GetKey()
    {
        return new UserExternalTokenKey(UserId, Provider);
    }
}
