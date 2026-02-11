using Directory.Application.DTOs;
using Directory.Application.Helpers;
using Directory.Application.Interfaces;
using Directory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Directory.Application.Services;

public class MemberService : IMemberService
{
    private readonly IApplicationDbContext _db;
    private readonly IUserNameResolver _userNameResolver;

    public MemberService(IApplicationDbContext db, IUserNameResolver userNameResolver)
    {
        _db = db;
        _userNameResolver = userNameResolver;
    }

    public async Task<List<MemberListDto>> GetDirectoryListAsync()
    {
        return await _db.Members
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .Select(m => new MemberListDto
            {
                Id = m.Id,
                FirstName = m.FirstName,
                LastName = m.LastName,
                CellPhone = m.CellPhone,
                HomePhone = m.HomePhone,
                LastUpdate = m.ModifiedDate
            })
            .ToListAsync();
    }

    public async Task<MemberDetailDto?> GetMemberDetailAsync(int id)
    {
        var member = await _db.Members
            .Include(m => m.Salutation)
            .Include(m => m.MaritalStatus)
            .Include(m => m.State)
            .Include(m => m.RelationshipsAsSource)
                .ThenInclude(r => r.RelatedMember)
                    .ThenInclude(rm => rm.Salutation)
            .Include(m => m.RelationshipsAsSource)
                .ThenInclude(r => r.RelationshipType)
            .Include(m => m.Notes)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null) return null;

        var dto = member.ToDetailDto();

        // Populate note user names
        foreach (var note in dto.Notes)
        {
            var memberNote = member.Notes.FirstOrDefault(n => n.Id == note.Id);
            if (memberNote != null)
            {
                note.UserName = await _userNameResolver.GetDisplayNameAsync(memberNote.UserId);
            }
        }

        return dto;
    }

    public async Task<MemberDto?> GetMemberForEditAsync(int id)
    {
        var member = await _db.Members
            .Include(m => m.Salutation)
            .Include(m => m.MaritalStatus)
            .Include(m => m.State)
            .Include(m => m.RelationshipsAsSource)
                .ThenInclude(r => r.RelatedMember)
                    .ThenInclude(rm => rm.Salutation)
            .Include(m => m.RelationshipsAsSource)
                .ThenInclude(r => r.RelationshipType)
            .Include(m => m.Notes)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null) return null;

        var dto = member.ToEditDto();

        foreach (var note in dto.Notes)
        {
            var memberNote = member.Notes.FirstOrDefault(n => n.Id == note.Id);
            if (memberNote != null)
            {
                note.UserName = await _userNameResolver.GetDisplayNameAsync(memberNote.UserId);
            }
        }

        return dto;
    }

    public async Task<int> CreateAsync(MemberDto dto, string? noteText, string userId)
    {
        var entity = new Member
        {
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
        dto.ApplyToEntity(entity);

        _db.Members.Add(entity);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(noteText))
        {
            _db.MemberNotes.Add(new MemberNote
            {
                MemberId = entity.Id,
                UserId = userId,
                NoteText = noteText,
                CreatedDate = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        // Save relationships
        if (dto.RelatedMembers.Count > 0)
        {
            await SaveRelationshipsAsync(entity.Id, dto.RelatedMembers);
        }

        return entity.Id;
    }

    public async Task UpdateAsync(MemberDto dto, string? noteText, string userId)
    {
        var entity = await _db.Members.FindAsync(dto.Id);
        if (entity == null) return;

        dto.ApplyToEntity(entity);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(noteText))
        {
            _db.MemberNotes.Add(new MemberNote
            {
                MemberId = entity.Id,
                UserId = userId,
                NoteText = noteText,
                CreatedDate = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        // Save relationships
        await SaveRelationshipsAsync(entity.Id, dto.RelatedMembers);
    }

    public async Task SaveRelationshipsAsync(int memberId, List<RelatedMemberDto> relationships)
    {
        // Delete all existing relationships for this member (both directions)
        var existing = await _db.MemberRelationships
            .Where(r => r.MemberId == memberId || r.RelatedMemberId == memberId)
            .ToListAsync();
        _db.MemberRelationships.RemoveRange(existing);

        // Insert new relationships with bidirectional mirroring
        foreach (var rel in relationships)
        {
            // Primary direction
            _db.MemberRelationships.Add(new MemberRelationship
            {
                MemberId = memberId,
                RelatedMemberId = rel.MemberId,
                RelationshipTypeId = rel.RelationshipTypeId
            });

            // Mirror/reciprocal relationship
            int reciprocalTypeId = rel.RelationshipTypeId switch
            {
                1 => 1, // Spouse → Spouse
                3 => 3, // Sibling → Sibling
                2 => 4, // Child → Parent
                4 => 2, // Parent → Child
                _ => rel.RelationshipTypeId
            };

            _db.MemberRelationships.Add(new MemberRelationship
            {
                MemberId = rel.MemberId,
                RelatedMemberId = memberId,
                RelationshipTypeId = reciprocalTypeId
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Members.FindAsync(id);
        if (entity == null) return;

        // Remove all relationships
        var relationships = await _db.MemberRelationships
            .Where(r => r.MemberId == id || r.RelatedMemberId == id)
            .ToListAsync();
        _db.MemberRelationships.RemoveRange(relationships);

        // Notes cascade delete via EF config
        _db.Members.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
