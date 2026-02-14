using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatRoleEnumDto
{
    [JsonStringEnumMemberName("system")] System = 1,

    [JsonStringEnumMemberName("assistant")]
    Assistant = 2,
    [JsonStringEnumMemberName("user")] User = 3,
    [JsonStringEnumMemberName("tool")] Tool = 4
}
