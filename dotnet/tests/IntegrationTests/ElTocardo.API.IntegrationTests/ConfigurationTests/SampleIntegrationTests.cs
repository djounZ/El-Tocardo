using ElTocardo.API.IntegrationTests.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ElTocardo.API.IntegrationTests.ConfigurationTests;

[Collection("Global test collection")]
public class SampleIntegrationTests
{
    private readonly IMongoDatabase _db;
    private readonly GlobalTestFixture _fixture;

    public SampleIntegrationTests(GlobalTestFixture fixture)
    {
        _fixture = fixture;

        var client = _fixture.ServiceProvider.GetRequiredService<IMongoClient>();
        _db = client.GetDatabase("TestDb");
    }

    [Fact]
    public async Task CanInsertIntoMongoDb()
    {
        var collection = _db.GetCollection<BsonDocument>("items");
        await collection.InsertOneAsync(new BsonDocument("name", "test item"));

        var count = await collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
        Assert.Equal(1, count);
    }
}
