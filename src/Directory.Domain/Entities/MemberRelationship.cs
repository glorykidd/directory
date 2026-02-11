namespace Directory.Domain.Entities;

public class MemberRelationship : BaseEntity
{
    public int MemberId { get; set; }
    public int RelatedMemberId { get; set; }
    public int RelationshipTypeId { get; set; }

    // Navigation properties
    public Member Member { get; set; } = null!;
    public Member RelatedMember { get; set; } = null!;
    public RelationshipType RelationshipType { get; set; } = null!;
}
