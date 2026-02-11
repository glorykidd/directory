using Directory.Application.DTOs;
using Directory.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Directory.Application.Services;

public class LookupService : ILookupService
{
    private readonly IApplicationDbContext _db;

    public LookupService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<LookupDto>> GetStatesAsync()
    {
        return await _db.States
            .OrderBy(s => s.Name)
            .Select(s => new LookupDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<List<LookupDto>> GetSalutationsAsync()
    {
        return await _db.Salutations
            .OrderBy(s => s.Id)
            .Select(s => new LookupDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<List<LookupDto>> GetMaritalStatusesAsync()
    {
        return await _db.MaritalStatuses
            .OrderBy(s => s.Id)
            .Select(s => new LookupDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<List<LookupDto>> GetRelationshipTypesAsync()
    {
        return await _db.RelationshipTypes
            .OrderBy(s => s.Id)
            .Select(s => new LookupDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<List<LookupDto>> GetAllMembersLookupAsync()
    {
        return await _db.Members
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .Select(m => new LookupDto
            {
                Id = m.Id,
                Name = (m.LastName + ", " + m.FirstName + (m.MiddleName != "" ? " " + m.MiddleName : "")).Trim()
            })
            .ToListAsync();
    }
}
