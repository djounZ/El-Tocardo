using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using Microsoft.AspNetCore.DataProtection;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.UserExternalTokenMediator;

public class UserExternalTokenProtector(IDataProtectionProvider provider)
{
    private readonly IDataProtector _protector = provider.CreateProtector(nameof(UserExternalToken));

    public string Protect(string token)
    {
        return _protector.Protect(token);
    }

    public string Unprotect(string protectedToken)
    {
        return _protector.Unprotect(protectedToken);
    }
}
