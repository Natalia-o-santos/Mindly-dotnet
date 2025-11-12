using Microsoft.EntityFrameworkCore;
using Mindly.Domain.Abstractions;
using Mindly.Domain.Entities;
using Mindly.Infrastructure.Persistence;

namespace Mindly.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly MindlyDbContext _dbContext;

    public UserProfileRepository(MindlyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserProfiles
            .Include(user => user.Sessions)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserProfiles
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserProfiles.AddAsync(userProfile, cancellationToken);
    }

    public Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        _dbContext.UserProfiles.Update(userProfile);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        _dbContext.UserProfiles.Remove(userProfile);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserProfiles.AnyAsync(user => user.Email == email, cancellationToken);
    }
}

