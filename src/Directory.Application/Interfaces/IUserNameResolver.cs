namespace Directory.Application.Interfaces;

public interface IUserNameResolver
{
    Task<string> GetDisplayNameAsync(string userId);
}
