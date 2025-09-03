using System.Text.Json.Serialization;

namespace AI.GithubCopilot.Infrastructure.Dtos.Authorizations;

public record GithubAccessTokenRequestDto
{
    [JsonPropertyName("client_id")] public string ClientId { get; init; } = string.Empty;

    [JsonPropertyName("device_code")] public string DeviceCode { get; init; } = string.Empty;

    [JsonPropertyName("grant_type")] public string TokenGrantType { get; init; } = string.Empty;
}
