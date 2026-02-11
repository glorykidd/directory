using Directory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Directory.Infrastructure.Data.Configurations;

public class MemberRelationshipConfiguration : IEntityTypeConfiguration<MemberRelationship>
{
    public void Configure(EntityTypeBuilder<MemberRelationship> builder)
    {
        builder.ToTable("MemberRelationships");
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.MemberId, r.RelatedMemberId }).IsUnique();

        builder.HasOne(r => r.Member)
            .WithMany(m => m.RelationshipsAsSource)
            .HasForeignKey(r => r.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RelatedMember)
            .WithMany(m => m.RelationshipsAsTarget)
            .HasForeignKey(r => r.RelatedMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RelationshipType)
            .WithMany()
            .HasForeignKey(r => r.RelationshipTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
