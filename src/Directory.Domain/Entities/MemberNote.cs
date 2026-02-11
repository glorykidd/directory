namespace Directory.Domain.Entities;

public class MemberNote : BaseEntity
{
    public int MemberId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string NoteText { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public Member Member { get; set; } = null!;
}
