using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Repositories;
using ElTocardo.Infrastructure.Mediator.Data;
using ElTocardo.Infrastructure.Mediator.Repositories;
using ElTocardo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ElTocardo.Application.IntegrationTests.PresetChatOptions;

public class PresetChatOptionsServiceIntegrationTests : IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _dbContext;

    public PresetChatOptionsServiceIntegrationTests(ITestOutputHelper output)
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
        );
        services.AddLogging();
        services.AddScoped<IPresetChatOptionsRepository, PresetChatOptionsRepository>();
        services.AddScoped<IPresetChatOptionsService, PresetChatOptionsService>();

        _serviceProvider = services.BuildServiceProvider();
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        loggerFactory.AddProvider(new TestOutputLoggerProvider(output));
        _dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreatePreset_WithValidData_ShouldReturnId()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsService>();
        var preset = new PresetChatOptionsDto("preset1", new ChatOptionsDto(
            ConversationId: "conv1",
            Instructions: "Say hello",
            Temperature: 0.5f,
            MaxOutputTokens: 100,
            TopP: 0.9f,
            TopK: 40,
            FrequencyPenalty: 0.1f,
            PresencePenalty: 0.2f,
            Seed: 42,
            ResponseFormat: null,
            ModelId: "gpt-4",
            StopSequences: new List<string> { "stop" },
            AllowMultipleToolCalls: false,
            ToolMode: null,
            Tools: null
        ));
        var id = await service.CreateAsync(preset);
        Assert.NotEqual(Guid.Empty, id);
    }


    [Fact]
    public async Task GetAllPresets_AfterCreating_ShouldReturnAll()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsService>();
        await service.CreateAsync(new PresetChatOptionsDto("p1", new ChatOptionsDto(null, "A", null, null, null, null, null, null, null, null, null, null, null, null, null)));
        await service.CreateAsync(new PresetChatOptionsDto("p2", new ChatOptionsDto(null, "B", null, null, null, null, null, null, null, null, null, null, null, null, null)));
        var all = (await service.GetAllAsync()).ToList();
        Assert.Equal(2, all.Count);
        Assert.Contains(all, x => x.Name == "p1");
        Assert.Contains(all, x => x.Name == "p2");
    }

    [Fact]
    public async Task UpdatePreset_ShouldChangeData()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsService>();
        var preset = new PresetChatOptionsDto("to-update", new ChatOptionsDto(null, "Old", null, null, null, null, null, null, null, null, null, null, null, null, null));
        await service.CreateAsync(preset);
        var updated = new PresetChatOptionsDto("to-update", new ChatOptionsDto(null, "New", null, null, null, null, null, null, null, null, null, null, null, null, null));
        var ok = await service.UpdateAsync("to-update", updated);
        Assert.True(ok);
        var fetched = await service.GetByNameAsync("to-update");
        Assert.Equal("New", fetched?.ChatOptions.Instructions);
    }

    [Fact]
    public async Task DeletePreset_ShouldRemove()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsService>();
        var preset = new PresetChatOptionsDto("to-delete", new ChatOptionsDto(null, "Del", null, null, null, null, null, null, null, null, null, null, null, null, null));
        await service.CreateAsync(preset);
        var ok = await service.DeleteAsync("to-delete");
        Assert.True(ok);
        var fetched = await service.GetByNameAsync("to-delete");
        Assert.Null(fetched);
    }

    [Fact]
    public async Task GetByName_NonExistent_ShouldReturnNull()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsService>();
        var fetched = await service.GetByNameAsync("not-exist");
        Assert.Null(fetched);
    }

    [Fact]
    public async Task Update_NonExistent_ShouldReturnFalse()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsService>();
        var updated = new PresetChatOptionsDto("not-exist", new ChatOptionsDto(null, "X", null, null, null, null, null, null, null, null, null, null, null, null, null));
        var ok = await service.UpdateAsync("not-exist", updated);
        Assert.False(ok);
    }

    [Fact]
    public async Task Delete_NonExistent_ShouldReturnFalse()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsService>();
        var ok = await service.DeleteAsync("not-exist");
        Assert.False(ok);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }
}
