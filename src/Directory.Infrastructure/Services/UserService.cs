using Directory.Application.DTOs;
using Directory.Application.Interfaces;
using Directory.Infrastructure.Data;
using Directory.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Directory.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _db.Users
            .Include(u => u.Member)
            .OrderBy(u => u.DisplayName)
            .ToListAsync();

        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                Role = roles.FirstOrDefault() ?? "User",
                MemberId = user.MemberId,
                MemberName = user.Member != null
                    ? $"{user.Member.LastName}, {user.Member.FirstName}"
                    : null,
                IsSuperAdmin = user.IsSuperAdmin
            });
        }
        return result;
    }

    public async Task<UserDto?> GetUserAsync(string id)
    {
        var user = await _db.Users.Include(u => u.Member).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            Role = roles.FirstOrDefault() ?? "User",
            MemberId = user.MemberId,
            MemberName = user.Member != null
                ? $"{user.Member.LastName}, {user.Member.FirstName}"
                : null,
            IsSuperAdmin = user.IsSuperAdmin
        };
    }

    public async Task<(bool Success, string? Error)> CreateAsync(UserDto dto, string password)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            MemberId = dto.MemberId,
            IsSuperAdmin = dto.Role == "SuperAdmin",
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, dto.Role);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(UserDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id);
        if (user == null) return (false, "User not found.");

        user.Email = dto.Email;
        user.UserName = dto.Email;
        user.DisplayName = dto.DisplayName;
        user.MemberId = dto.MemberId;
        user.IsSuperAdmin = dto.Role == "SuperAdmin";

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        // Update role
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.Role);

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return (false, "User not found.");

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<(bool Success, string TempPassword, string? Error)> ResetPasswordAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return (false, string.Empty, "User not found.");

        var tempPassword = GenerateTemporaryPassword();
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, tempPassword);

        return result.Succeeded
            ? (true, tempPassword, null)
            : (false, string.Empty, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<(bool Success, string? Error)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return (false, "User not found.");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    private static string GenerateTemporaryPassword()
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%";

        var random = new Random();
        var password = new char[10];

        // Ensure at least one of each required type
        password[0] = upper[random.Next(upper.Length)];
        password[1] = lower[random.Next(lower.Length)];
        password[2] = digits[random.Next(digits.Length)];
        password[3] = special[random.Next(special.Length)];

        var all = upper + lower + digits + special;
        for (int i = 4; i < 10; i++)
        {
            password[i] = all[random.Next(all.Length)];
        }

        // Shuffle
        for (int i = password.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password);
    }
}
