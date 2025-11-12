using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindly.Domain.Entities;

namespace Mindly.Infrastructure.Persistence.Configurations;

public class FocusSessionConfiguration : IEntityTypeConfiguration<FocusSession>
{
    public void Configure(EntityTypeBuilder<FocusSession> builder)
    {
        builder.ToTable("FocusSessions");

        builder.HasKey(session => session.Id);

        builder.Property(session => session.Title)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(session => session.Description)
            .HasMaxLength(500);

        builder.Property(session => session.FocusMode)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(session => session.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property<int>("_plannedDurationMinutes")
            .HasColumnName("PlannedDurationMinutes")
            .IsRequired();

        builder.Property<int?>("_actualDurationMinutes")
            .HasColumnName("ActualDurationMinutes");

        builder.Property(session => session.CancellationReason)
            .HasMaxLength(300);

        builder.HasMany(session => session.Breaks)
            .WithOne()
            .HasForeignKey(breakPeriod => breakPeriod.FocusSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(session => session.DeviceSignals)
            .WithOne()
            .HasForeignKey(signal => signal.FocusSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(session => session.Breaks)
            .HasField("_breaks")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(session => session.DeviceSignals)
            .HasField("_deviceSignals")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

