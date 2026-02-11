using Directory.Domain.Enums;

namespace Directory.Application.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    public int SalutationId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public int MaritalStatusId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? MarriageDate { get; set; }
    public string Address1 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int StateId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string CellPhone { get; set; } = string.Empty;
    public string HomePhone { get; set; } = string.Empty;
    public string Email1 { get; set; } = string.Empty;
    public string Email2 { get; set; } = string.Empty;
    public List<RelatedMemberDto> RelatedMembers { get; set; } = new();
    public List<MemberNoteDto> Notes { get; set; } = new();
}
