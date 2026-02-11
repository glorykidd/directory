using Directory.Application.DTOs;

namespace Directory.Application.Interfaces;

public interface ISystemSettingService
{
    Task<SystemSettingDto> GetSettingsAsync();
    Task UpdateSettingsAsync(SystemSettingDto dto);
}
