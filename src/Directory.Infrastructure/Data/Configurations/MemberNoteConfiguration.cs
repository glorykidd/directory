using Directory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Directory.Infrastructure.Data.Configurations;

public class MemberNoteConfiguration : IEntityTypeConfiguration<MemberNote>
{
    public void Configure(EntityTypeBuilder<MemberNote> builder)
    {
        builder.ToTable("MemberNotes");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.NoteText).IsRequired();
        builder.Property(n => n.UserId).HasMaxLength(450).IsRequired();

        builder.HasOne(n => n.Member)
            .WithMany(m => m.Notes)
            .HasForeignKey(n => n.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
