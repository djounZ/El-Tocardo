namespace ElTocardo.API.Options;

public record OutputCachingPolicy( TimeSpan? ExpireTimeSpan = null ,string? VaryByHeader = null);
