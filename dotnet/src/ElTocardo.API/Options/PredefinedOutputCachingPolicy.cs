namespace ElTocardo.API.Options;

public static class PredefinedOutputCachingPolicy
{
    public const string PerUserVaryByHeaderAuthorizationShortLiving =  nameof(PerUserVaryByHeaderAuthorizationShortLiving);
    public const string PerUserVaryByHeaderAuthorizationLongLiving = nameof(PerUserVaryByHeaderAuthorizationLongLiving);


    public const string GlobalShortLiving = nameof(GlobalShortLiving);
    public const string GlobalLongLiving = nameof(GlobalLongLiving);

}