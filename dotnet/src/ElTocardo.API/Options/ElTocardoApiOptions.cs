namespace ElTocardo.API.Options;

public class ElTocardoApiOptions
{
    public string?  McpServerConfigurationFile { get; set; }
    public IDictionary<string,OutputCachingPolicy> OutputCachingPolicies { get; set; } = new Dictionary<string, OutputCachingPolicy>();

    public OpenIddictServerOptions OpenIddictServerOptions { get; set; } = new();
}


public class OpenIddictServerOptions
{
    public string TokenEndpointUri   { get; set; } = string.Empty;
    public string AuthorizationEndpointUri   { get; set; } = string.Empty;
    public string EndSessionEndpointUri    { get; set; } = string.Empty;
    public string UserInfoEndpointUri    { get; set; } = string.Empty;
    public string IntrospectionEndpointUri    { get; set; } = string.Empty;
}
