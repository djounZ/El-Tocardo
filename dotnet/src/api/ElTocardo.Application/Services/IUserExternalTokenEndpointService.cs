using ElTocardo.Application.Dtos.UserExternalToken;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;

public interface IUserExternalTokenEndpointService
{
    public Task<Result<Dictionary<UserExternalTokenKey, UserExternalTokenItemDto>>> GetAllUserExternalTokensAsync(
        CancellationToken cancellationToken = default);

    public Task<Result<UserExternalTokenItemDto>> GetUserExternalTokenAsync(string userid, string provider,
        CancellationToken cancellationToken = default);

    public Task<VoidResult> CreateUserExternalTokenAsync(string userId, string provider, string token,
        CancellationToken cancellationToken = default);

    public Task<VoidResult> UpdateServerAsync(string userId, string provider, string token,
        CancellationToken cancellationToken = default);

    public Task<VoidResult> DeleteServerAsync(string userId, string provider, CancellationToken cancellationToken = default);
}
