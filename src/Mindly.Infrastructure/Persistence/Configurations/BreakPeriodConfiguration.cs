using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindly.Domain.Entities;

namespace Mindly.Infrastructure.Persistence.Configurations;

public class BreakPeriodConfiguration : IEntityTypeConfiguration<BreakPeriod>
{
    public void Configure(EntityTypeBuilder<BreakPeriod> builder)
    {
        builder.ToTable("BreakPeriods");

        builder.HasKey(breakPeriod => breakPeriod.Id);

        builder.Property(breakPeriod => breakPeriod.BreakType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(breakPeriod => breakPeriod.Notes)
            .HasMaxLength(200);

        builder.Property<int>("_durationMinutes")
            .HasColumnName("DurationMinutes")
            .IsRequired();
    }
}

