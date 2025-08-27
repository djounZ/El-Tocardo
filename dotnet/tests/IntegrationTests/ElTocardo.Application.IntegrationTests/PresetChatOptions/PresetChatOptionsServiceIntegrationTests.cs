using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Queries;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Queries;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Validators;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Data;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Repositories;
using ElTocardo.Infrastructure.Services;
using ElTocardo.Infrastructure.Services.Endpoints;
using FluentValidation;
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
        
        // Add DbContext with in-memory database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
        );
        
        // Add logging
        services.AddLogging();
        
        // Register services manually for testing
        services.AddScoped<IPresetChatOptionsEndpointService, PresetChatOptionsEndpointService>();
        
        // Add handlers and repositories (simplified for test)
        services.AddSingleton<PresetChatOptionsDomainGetDtoMapper>();
        services.AddSingleton<PresetChatOptionsDomainGetAllDtoMapper>();
        services.AddSingleton<PresetChatOptionsDomainUpdateCommandMapper>();
        services.AddSingleton<PresetChatOptionsDomainCreateCommandMapper>();
        services.AddScoped<IPresetChatOptionsRepository, PresetChatOptionsRepository>();
        
        services.AddScoped<ICommandHandler<CreatePresetChatOptionsCommand, Guid>, CreatePresetChatOptionsCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePresetChatOptionsCommand>, UpdatePresetChatOptionsCommandHandler>();
        services.AddScoped<ICommandHandler<DeletePresetChatOptionsCommand>, DeletePresetChatOptionsCommandHandler>();
        services
            .AddScoped<IQueryHandler<GetAllPresetChatOptionsQuery, List<PresetChatOptionsDto>>,
                GetAllPresetChatOptionsQueryHandler>();
        services
            .AddScoped<IQueryHandler<GetPresetChatOptionsByNameQuery, PresetChatOptionsDto>,
                GetPresetChatOptionsByNameQueryHandler>();

        // Add validators
        services.AddScoped<IValidator<CreatePresetChatOptionsCommand>, CreatePresetChatOptionsCommandValidator>();
        services.AddScoped<IValidator<UpdatePresetChatOptionsCommand>, UpdatePresetChatOptionsCommandValidator>();
        services.AddScoped<IValidator<DeletePresetChatOptionsCommand>, DeletePresetChatOptionsCommandValidator>();

        _serviceProvider = services.BuildServiceProvider();
        
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        loggerFactory.AddProvider(new TestOutputLoggerProvider(output));
        _dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created
        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreatePreset_WithValidData_ShouldReturnSuccess()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
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
        
        var result = await service.CreateAsync(preset);
        
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.ReadValue());
    }

    [Fact]
    public async Task GetAllPresets_AfterCreating_ShouldReturnAll()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
        await service.CreateAsync(new PresetChatOptionsDto("p1", new ChatOptionsDto(null, "A", null, null, null, null, null, null, null, null, null, null, null, null, null)));
        await service.CreateAsync(new PresetChatOptionsDto("p2", new ChatOptionsDto(null, "B", null, null, null, null, null, null, null, null, null, null, null, null, null)));
        
        var result = await service.GetAllAsync();
        
        Assert.True(result.IsSuccess);
        var all = result.ReadValue();
        Assert.Equal(2, all.Count);
        Assert.Contains(all, x => x.Name == "p1");
        Assert.Contains(all, x => x.Name == "p2");
    }

    [Fact]
    public async Task UpdatePreset_ShouldChangeData()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
        var preset = new PresetChatOptionsDto("to-update", new ChatOptionsDto(null, "Old", null, null, null, null, null, null, null, null, null, null, null, null, null));
        await service.CreateAsync(preset);
        var updated = new PresetChatOptionsDto("to-update", new ChatOptionsDto(null, "New", null, null, null, null, null, null, null, null, null, null, null, null, null));
        
        var updateResult = await service.UpdateAsync("to-update", updated);
        
        Assert.True(updateResult.IsSuccess);
        var fetchResult = await service.GetByNameAsync("to-update");
        Assert.True(fetchResult.IsSuccess);
        Assert.Equal("New", fetchResult.ReadValue().ChatOptions.Instructions);
    }

    [Fact]
    public async Task DeletePreset_ShouldRemove()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
        var preset = new PresetChatOptionsDto("to-delete", new ChatOptionsDto(null, "Del", null, null, null, null, null, null, null, null, null, null, null, null, null));
        await service.CreateAsync(preset);
        
        var deleteResult = await service.DeleteAsync("to-delete");
        
        Assert.True(deleteResult.IsSuccess);
        var fetchResult = await service.GetByNameAsync("to-delete");
        Assert.False(fetchResult.IsSuccess);
    }

    [Fact]
    public async Task GetByName_NonExistent_ShouldReturnFailure()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
        
        var result = await service.GetByNameAsync("not-exist");
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Update_NonExistent_ShouldReturnFailure()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
        var updated = new PresetChatOptionsDto("not-exist", new ChatOptionsDto(null, "X", null, null, null, null, null, null, null, null, null, null, null, null, null));
        
        var result = await service.UpdateAsync("not-exist", updated);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_NonExistent_ShouldReturnFailure()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
        
        var result = await service.DeleteAsync("not-exist");
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreatePreset_WithInvalidName_ShouldReturnFailure()
    {
        var service = _serviceProvider.GetRequiredService<IPresetChatOptionsEndpointService>();
        var preset = new PresetChatOptionsDto("", new ChatOptionsDto(null, "Test", null, null, null, null, null, null, null, null, null, null, null, null, null));
        
        var result = await service.CreateAsync(preset);
        
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.ReadError().Message);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }
}
