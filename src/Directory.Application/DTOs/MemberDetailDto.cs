namespace Directory.Application.DTOs;

public class MemberDetailDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string FormattedAddress { get; set; } = string.Empty;
    public string CellPhone { get; set; } = string.Empty;
    public string HomePhone { get; set; } = string.Empty;
    public string Email1 { get; set; } = string.Empty;
    public string Email2 { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string MarriageDate { get; set; } = string.Empty;
    public List<RelatedMemberDto> RelatedMembers { get; set; } = new();
    public List<MemberNoteDto> Notes { get; set; } = new();
}
