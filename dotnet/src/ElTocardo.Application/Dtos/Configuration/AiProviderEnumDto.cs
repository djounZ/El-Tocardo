using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Configuration;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AiProviderEnumDto
{
    [JsonStringEnumMemberName("github_copilot")]
    GithubCopilot = 1,

    [JsonStringEnumMemberName("ollama")] Ollama = 2 /*,
    OpenAI = 3,
    Anthropic = 4*/
}
