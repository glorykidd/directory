namespace Directory.Application.DTOs;

public class RelatedMemberDto
{
    public int MemberId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int RelationshipTypeId { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
}
