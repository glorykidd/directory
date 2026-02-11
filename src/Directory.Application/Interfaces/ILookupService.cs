using Directory.Application.DTOs;

namespace Directory.Application.Interfaces;

public interface ILookupService
{
    Task<List<LookupDto>> GetStatesAsync();
    Task<List<LookupDto>> GetSalutationsAsync();
    Task<List<LookupDto>> GetMaritalStatusesAsync();
    Task<List<LookupDto>> GetRelationshipTypesAsync();
    Task<List<LookupDto>> GetAllMembersLookupAsync();
}
