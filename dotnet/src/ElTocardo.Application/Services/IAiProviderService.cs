using ElTocardo.Application.Dtos.Provider;

namespace ElTocardo.Application.Services;

public interface IAiProviderService
{
    public Task<AiProviderDto[]> GetAllAsync(CancellationToken cancellationToken);
    public Task<AiProviderDto?> GetAsync(AiProviderEnumDto provider, CancellationToken cancellationToken);
}
