namespace ElTocardo.API.IntegrationTests.Configuration;

[CollectionDefinition("Global test collection")]
public class GlobalTestCollection : ICollectionFixture<GlobalTestFixture>
{
    // No code here – xUnit uses this for tagging and shared setup
}
