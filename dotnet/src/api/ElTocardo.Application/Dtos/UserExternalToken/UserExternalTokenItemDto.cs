namespace ElTocardo.Application.Dtos.UserExternalToken;

public record UserExternalTokenItemDto(
    string UserId,
    string Provider,
    string Value,
    DateTime CreatedAt,
    DateTime UpdatedAt);
