using Directory.Application.DTOs;

namespace Directory.Application.Interfaces;

public interface ISystemExceptionService
{
    Task<List<SystemExceptionDto>> GetAllAsync();
    Task LogAsync(string module, string message, string? stackTrace);
}
