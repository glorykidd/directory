using Directory.Application.DTOs;
using Directory.Application.Helpers;
using Directory.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Directory.Application.Services;

public class SystemSettingService : ISystemSettingService
{
    private readonly IApplicationDbContext _db;

    public SystemSettingService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<SystemSettingDto> GetSettingsAsync()
    {
        var entity = await _db.SystemSettings.FirstOrDefaultAsync();
        if (entity == null)
        {
            return new SystemSettingDto();
        }
        return entity.ToDto();
    }

    public async Task UpdateSettingsAsync(SystemSettingDto dto)
    {
        var entity = await _db.SystemSettings.FirstOrDefaultAsync();
        if (entity == null)
        {
            entity = new Domain.Entities.SystemSetting();
            _db.SystemSettings.Add(entity);
        }
        dto.ApplyToEntity(entity);
        await _db.SaveChangesAsync();
    }
}
