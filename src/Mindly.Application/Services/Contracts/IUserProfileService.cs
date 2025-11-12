using Mindly.Application.DTOs.Users;

namespace Mindly.Application.Services.Contracts;

public interface IUserProfileService
{
    Task<UserProfileDto> CreateAsync(CreateUserProfileRequest request, CancellationToken cancellationToken = default);
    Task<UserProfileDto> UpdateAsync(Guid id, UpdateUserProfileRequest request, CancellationToken cancellationToken = default);
    Task<UserProfileDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

