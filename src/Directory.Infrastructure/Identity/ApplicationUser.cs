using Directory.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Directory.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public int? MemberId { get; set; }
    public bool IsSuperAdmin { get; set; }

    // Navigation
    public Member? Member { get; set; }
}
