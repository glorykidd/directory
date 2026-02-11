using Directory.Domain.Enums;

namespace Directory.Domain.Entities;

public class Member : BaseEntity
{
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
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public Salutation Salutation { get; set; } = null!;
    public MaritalStatus MaritalStatus { get; set; } = null!;
    public State State { get; set; } = null!;
    public ICollection<MemberRelationship> RelationshipsAsSource { get; set; } = new List<MemberRelationship>();
    public ICollection<MemberRelationship> RelationshipsAsTarget { get; set; } = new List<MemberRelationship>();
    public ICollection<MemberNote> Notes { get; set; } = new List<MemberNote>();

    public string DisplayName
    {
        get
        {
            var parts = new List<string>();
            if (Salutation != null && !string.IsNullOrEmpty(Salutation.Name))
                parts.Add(Salutation.Name);
            if (!string.IsNullOrEmpty(FirstName))
                parts.Add(FirstName);
            if (!string.IsNullOrEmpty(MiddleName))
                parts.Add(MiddleName);
            if (!string.IsNullOrEmpty(LastName))
                parts.Add(LastName);
            if (!string.IsNullOrEmpty(Suffix))
                parts.Add(Suffix);
            return string.Join(" ", parts);
        }
    }
}
