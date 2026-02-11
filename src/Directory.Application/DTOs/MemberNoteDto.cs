namespace Directory.Application.DTOs;

public class MemberNoteDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string NoteText { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
