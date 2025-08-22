namespace ElTocardo.Infrastructure.Options;

public sealed class ElTocardoInfrastructureOptions
{
    public string?  McpServerConfigurationFile { get; set; }

    public JwtBearerTokenValidationParametersOptions JwtBearerTokenValidationParametersOptions { get; set; } = new();
}

public class JwtBearerTokenValidationParametersOptions
{
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience  { get; set; } = true;
    public bool ValidateLifetime  { get; set; } = true;
    public bool ValidateIssuerSigningKey  { get; set; } = true;
    public string ValidIssuer   { get; set; } = string.Empty;
    public string ValidAudience   { get; set; } = string.Empty;
    public string Secret    { get; set; } = string.Empty;
}
