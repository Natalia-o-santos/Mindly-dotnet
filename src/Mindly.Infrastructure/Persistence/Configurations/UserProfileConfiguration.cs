using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindly.Domain.Entities;

namespace Mindly.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(180)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.WorkMode)
            .HasConversion<int>()
            .IsRequired();

        builder.Property<int>("_dailyFocusGoalMinutes")
            .HasColumnName("DailyFocusGoalMinutes")
            .IsRequired();

        builder.HasMany(user => user.Sessions)
            .WithOne()
            .HasForeignKey(session => session.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(user => user.Sessions)
            .HasField("_sessions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

