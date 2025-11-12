using Mindly.Application.DTOs.Users;
using Mindly.Application.Exceptions;
using Mindly.Application.Mappings;
using Mindly.Application.Services.Contracts;
using Mindly.Domain.Abstractions;
using Mindly.Domain.Entities;
using Mindly.Domain.ValueObjects;

namespace Mindly.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserProfileService(IUserProfileRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto> CreateAsync(CreateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateUserUniquenessAsync(request.Email, cancellationToken);

        var user = new UserProfile(
            request.Name,
            request.Email,
            request.WorkMode,
            Duration.FromMinutes(request.DailyFocusGoalMinutes));

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserProfileMapper.ToDto(user);
    }

    public async Task<UserProfileDto> UpdateAsync(Guid id, UpdateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException("Perfil não encontrado.");

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            await ValidateUserUniquenessAsync(request.Email, cancellationToken);
        }

        user.SetName(request.Name);
        user.SetEmail(request.Email);
        user.ChangeWorkMode(request.WorkMode);
        user.UpdateDailyFocusGoal(Duration.FromMinutes(request.DailyFocusGoalMinutes));

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserProfileMapper.ToDto(user);
    }

    public async Task<UserProfileDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException("Perfil não encontrado.");

        return UserProfileMapper.ToDto(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException("Perfil não encontrado.");

        await _userRepository.DeleteAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateUserUniquenessAsync(string email, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (exists)
        {
            throw new ConflictException("Já existe um perfil cadastrado com este e-mail.");
        }
    }
}

