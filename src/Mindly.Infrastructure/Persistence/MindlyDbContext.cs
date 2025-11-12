using Microsoft.EntityFrameworkCore;
using Mindly.Domain.Entities;
using Mindly.Infrastructure.Persistence.Configurations;

namespace Mindly.Infrastructure.Persistence;

public class MindlyDbContext : DbContext
{
    public MindlyDbContext(DbContextOptions<MindlyDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<FocusSession> FocusSessions => Set<FocusSession>();
    public DbSet<BreakPeriod> BreakPeriods => Set<BreakPeriod>();
    public DbSet<DeviceSignal> DeviceSignals => Set<DeviceSignal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
        modelBuilder.ApplyConfiguration(new FocusSessionConfiguration());
        modelBuilder.ApplyConfiguration(new BreakPeriodConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceSignalConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}

