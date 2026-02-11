using Directory.Application.Interfaces;
using Directory.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Directory.Infrastructure.Services;

public class UserNameResolver : IUserNameResolver
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserNameResolver(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GetDisplayNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.DisplayName ?? "Unknown";
    }
}
