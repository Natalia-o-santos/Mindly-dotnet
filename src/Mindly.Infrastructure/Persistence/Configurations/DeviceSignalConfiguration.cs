using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindly.Domain.Entities;

namespace Mindly.Infrastructure.Persistence.Configurations;

public class DeviceSignalConfiguration : IEntityTypeConfiguration<DeviceSignal>
{
    public void Configure(EntityTypeBuilder<DeviceSignal> builder)
    {
        builder.ToTable("DeviceSignals");

        builder.HasKey(signal => signal.Id);

        builder.Property(signal => signal.DeviceName)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(signal => signal.SignalType)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(signal => signal.Payload)
            .HasMaxLength(200)
            .IsRequired();
    }
}

