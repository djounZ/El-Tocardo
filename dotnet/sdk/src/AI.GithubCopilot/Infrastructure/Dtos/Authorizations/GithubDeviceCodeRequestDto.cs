using System.Text.Json.Serialization;

namespace AI.GithubCopilot.Infrastructure.Dtos.Authorizations;

public class GithubDeviceCodeRequestDto
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; init; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scope { get; init; } = string.Empty;
}