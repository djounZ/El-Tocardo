using ElTocardo.API.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace ElTocardo.API.IntegrationTests.Configuration;

public class GlobalTestFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:8.0")
        .Build();

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17.5")
        .Build();

    public IConfiguration Configuration { get; private set; } = null!;
    public ServiceProvider ServiceProvider { get; private set; } = null!;


    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _mongoContainer.StartAsync();

        var mongoDbConnectionString = _mongoContainer.GetConnectionString();
        var postgresDbConnectionString = _postgreSqlContainer.GetConnectionString();

        // Build configuration for test
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:el-tocardo-db-mongodb"] = mongoDbConnectionString,
                ["ConnectionStrings:el-tocardo-db-postgres"] = postgresDbConnectionString
            }!)
            .Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(Configuration);
        serviceCollection.AddElTocardoApi(Configuration, "ElTocardo.API.IntegrationTests");
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        await ServiceProvider.DisposeAsync();
        await _mongoContainer.DisposeAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
}
