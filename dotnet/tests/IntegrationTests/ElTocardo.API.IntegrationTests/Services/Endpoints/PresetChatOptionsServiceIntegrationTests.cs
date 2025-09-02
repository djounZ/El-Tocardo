using ElTocardo.API.IntegrationTests.Configuration;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.API.IntegrationTests.Services.Endpoints;

[Collection("Global test collection")]
public class PresetChatOptionsServiceIntegrationTests : AbstractDbSetServiceIntegrationTests<PresetChatOptions>
{
    private readonly IPresetChatOptionsEndpointService _presetChatOptionsEndpointService;

    public PresetChatOptionsServiceIntegrationTests(GlobalTestFixture fixture) :
        base(fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>().PresetChatOptions)
    {
        var serviceProvider = fixture.ServiceProvider;


        _presetChatOptionsEndpointService = serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
    }

    [Fact]
    public async Task CreatePreset_WithValidData_ShouldReturnSuccess()
    {
        var preset = new PresetChatOptionsDto("preset1", new ChatOptionsDto(
            "conv1",
            "Say hello",
            0.5f,
            100,
            0.9f,
            40,
            0.1f,
            0.2f,
            42,
            null,
            "gpt-4",
            new List<string> { "stop" },
            false,
            null,
            null
        ));

        var result = await _presetChatOptionsEndpointService.CreateAsync(preset);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllPresets_AfterCreating_ShouldReturnAll()
    {
        await _presetChatOptionsEndpointService.CreateAsync(new PresetChatOptionsDto("p1",
            new ChatOptionsDto(null, "A", null, null, null, null, null, null, null, null, null, null, null, null,
                null)));
        await _presetChatOptionsEndpointService.CreateAsync(new PresetChatOptionsDto("p2",
            new ChatOptionsDto(null, "B", null, null, null, null, null, null, null, null, null, null, null, null,
                null)));

        var result = await _presetChatOptionsEndpointService.GetAllAsync();

        Assert.True(result.IsSuccess);
        var all = result.ReadValue();
        Assert.Equal(2, all.Count);
        Assert.Contains(all, x => x.Name == "p1");
        Assert.Contains(all, x => x.Name == "p2");
    }

    [Fact]
    public async Task UpdatePreset_ShouldChangeData()
    {
        var preset = new PresetChatOptionsDto("to-update",
            new ChatOptionsDto(null, null, 0.2f, null, null, null, null, null, null, null, null, null, null, null,
                null));
        await _presetChatOptionsEndpointService.CreateAsync(preset);
        var updated = new PresetChatOptionsDto("to-update",
            new ChatOptionsDto(null, null, 0.7f, null, null, null, null, null, null, null, null, null, null, null,
                null));

        var updateResult = await _presetChatOptionsEndpointService.UpdateAsync("to-update", updated);

        Assert.True(updateResult.IsSuccess);
        var fetchResult = await _presetChatOptionsEndpointService.GetByNameAsync("to-update");
        Assert.True(fetchResult.IsSuccess);
        Assert.Equal(0.7f, fetchResult.ReadValue().ChatOptions.Temperature);
    }

    [Fact]
    public async Task DeletePreset_ShouldRemove()
    {
        var preset = new PresetChatOptionsDto("to-delete",
            new ChatOptionsDto(null, "Del", null, null, null, null, null, null, null, null, null, null, null, null,
                null));
        await _presetChatOptionsEndpointService.CreateAsync(preset);

        var deleteResult = await _presetChatOptionsEndpointService.DeleteAsync("to-delete");

        Assert.True(deleteResult.IsSuccess);
        var fetchResult = await _presetChatOptionsEndpointService.GetByNameAsync("to-delete");
        Assert.False(fetchResult.IsSuccess);
    }

    [Fact]
    public async Task GetByName_NonExistent_ShouldReturnFailure()
    {
        var result = await _presetChatOptionsEndpointService.GetByNameAsync("not-exist");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Update_NonExistent_ShouldReturnFailure()
    {
        var updated = new PresetChatOptionsDto("not-exist",
            new ChatOptionsDto(null, "X", null, null, null, null, null, null, null, null, null, null, null, null,
                null));

        var result = await _presetChatOptionsEndpointService.UpdateAsync("not-exist", updated);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_NonExistent_ShouldReturnFailure()
    {
        var result = await _presetChatOptionsEndpointService.DeleteAsync("not-exist");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreatePreset_WithInvalidName_ShouldReturnFailure()
    {
        var preset = new PresetChatOptionsDto("",
            new ChatOptionsDto(null, "Test", null, null, null, null, null, null, null, null, null, null, null, null,
                null));

        var result = await _presetChatOptionsEndpointService.CreateAsync(preset);

        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.ReadError().Message);
    }
}
