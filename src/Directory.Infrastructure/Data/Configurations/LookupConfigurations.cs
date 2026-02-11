using Directory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Directory.Infrastructure.Data.Configurations;

public class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("States");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Abbreviation).HasMaxLength(2).IsRequired();
    }
}

public class SalutationConfiguration : IEntityTypeConfiguration<Salutation>
{
    public void Configure(EntityTypeBuilder<Salutation> builder)
    {
        builder.ToTable("Salutations");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(20).IsRequired();
    }
}

public class MaritalStatusConfiguration : IEntityTypeConfiguration<MaritalStatus>
{
    public void Configure(EntityTypeBuilder<MaritalStatus> builder)
    {
        builder.ToTable("MaritalStatuses");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(20).IsRequired();
    }
}

public class RelationshipTypeConfiguration : IEntityTypeConfiguration<RelationshipType>
{
    public void Configure(EntityTypeBuilder<RelationshipType> builder)
    {
        builder.ToTable("RelationshipTypes");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(20).IsRequired();
    }
}
