namespace ElTocardo.Authorization.Server.Options;

public class ElTocardoAuthorizationServerOptions
{

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
