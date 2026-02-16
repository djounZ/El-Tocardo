using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum McpClientRoleDto
{
    [JsonStringEnumMemberName("assistant")]
    Assistant = 2,
    [JsonStringEnumMemberName("user")] Tool = 4
}