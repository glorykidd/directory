using Directory.Application.DTOs;
using Directory.Application.Helpers;
using Directory.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Directory.Application.Services;

public class SystemExceptionService : ISystemExceptionService
{
    private readonly IApplicationDbContext _db;

    public SystemExceptionService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SystemExceptionDto>> GetAllAsync()
    {
        return await _db.SystemExceptions
            .OrderByDescending(e => e.Timestamp)
            .Select(e => e.ToDto())
            .ToListAsync();
    }

    public async Task LogAsync(string module, string message, string? stackTrace)
    {
        _db.SystemExceptions.Add(new Domain.Entities.SystemException
        {
            Timestamp = DateTime.UtcNow,
            Module = module,
            ExceptionMessage = message,
            StackTrace = stackTrace ?? string.Empty
        });
        await _db.SaveChangesAsync();
    }
}
