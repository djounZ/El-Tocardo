namespace ElTocardo.API.Options;

public class ElTocardoApiOptions
{
    public IDictionary<string,OutputCachingPolicy> OutputCachingPolicies { get; set; } = new Dictionary<string, OutputCachingPolicy>();

}
