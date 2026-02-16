using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum McpServerTransportTypeDto
{
    [JsonStringEnumMemberName("stdio")] Stdio = 1,
    [JsonStringEnumMemberName("http")] Http = 2
}