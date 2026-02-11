namespace Directory.Application.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? MemberId { get; set; }
    public string? MemberName { get; set; }
    public bool IsSuperAdmin { get; set; }
}
