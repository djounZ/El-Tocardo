using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;

namespace ElTocardo.API.Options;

public class OutputCacheOptionsSetup(IOptions<ElTocardoApiOptions> apiOptions) : IConfigureOptions<OutputCacheOptions>
{
    public void Configure(OutputCacheOptions options)
    {
        foreach (var (key, value) in apiOptions.Value.OutputCachingPolicies)
        {
            AddPolicy(options,key, value);
        }
    }

    private static void AddPolicy( OutputCacheOptions option, string key, OutputCachingPolicy value)
    {
        option.AddPolicy(key, builder =>
        {
            if (value.ExpireTimeSpan.HasValue)
            {
                builder.Expire(value.ExpireTimeSpan.Value);
            }
            if(value.VaryByHeader != null)
            {
                builder.SetVaryByHeader(value.VaryByHeader);
            }
        });
    }
}
