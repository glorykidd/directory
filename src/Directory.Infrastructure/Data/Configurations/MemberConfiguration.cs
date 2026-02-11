using Directory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Directory.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(m => m.MiddleName).HasMaxLength(100);
        builder.Property(m => m.LastName).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Suffix).HasMaxLength(20);
        builder.Property(m => m.Address1).HasMaxLength(200);
        builder.Property(m => m.Address2).HasMaxLength(200);
        builder.Property(m => m.City).HasMaxLength(100);
        builder.Property(m => m.ZipCode).HasMaxLength(10);
        builder.Property(m => m.CellPhone).HasMaxLength(10);
        builder.Property(m => m.HomePhone).HasMaxLength(10);
        builder.Property(m => m.Email1).HasMaxLength(200);
        builder.Property(m => m.Email2).HasMaxLength(200);

        builder.HasOne(m => m.Salutation)
            .WithMany()
            .HasForeignKey(m => m.SalutationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.MaritalStatus)
            .WithMany()
            .HasForeignKey(m => m.MaritalStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.State)
            .WithMany()
            .HasForeignKey(m => m.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(m => m.DisplayName);
    }
}
