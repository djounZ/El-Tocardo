using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ElTocardo.Infrastructure.Options;

public sealed class ConfigureJwtBearerOptions(IOptions<ElTocardoInfrastructureOptions> infrastructureOptions) :  IConfigureNamedOptions<JwtBearerOptions>
{
    private  JwtBearerTokenValidationParametersOptions JwtBearerTokenValidationParametersOptions => infrastructureOptions.Value?.JwtBearerTokenValidationParametersOptions!;

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = JwtBearerTokenValidationParametersOptions.ValidateIssuer,
            ValidateAudience = JwtBearerTokenValidationParametersOptions.ValidateAudience,
            ValidateLifetime = JwtBearerTokenValidationParametersOptions.ValidateLifetime,
            ValidateIssuerSigningKey = JwtBearerTokenValidationParametersOptions.ValidateIssuerSigningKey,
            ValidIssuer = JwtBearerTokenValidationParametersOptions.ValidIssuer,
            ValidAudience = JwtBearerTokenValidationParametersOptions.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtBearerTokenValidationParametersOptions.Secret))
        };
    }

    public void Configure(JwtBearerOptions options) => Configure(null, options);
}
