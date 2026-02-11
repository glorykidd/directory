using Directory.Application.DTOs;

namespace Directory.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserAsync(string id);
    Task<(bool Success, string? Error)> CreateAsync(UserDto dto, string password);
    Task<(bool Success, string? Error)> UpdateAsync(UserDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(string id);
    Task<(bool Success, string TempPassword, string? Error)> ResetPasswordAsync(string id);
    Task<(bool Success, string? Error)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
