using Directory.Application.DTOs;

namespace Directory.Application.Interfaces;

public interface IMemberService
{
    Task<List<MemberListDto>> GetDirectoryListAsync();
    Task<MemberDetailDto?> GetMemberDetailAsync(int id);
    Task<MemberDto?> GetMemberForEditAsync(int id);
    Task<int> CreateAsync(MemberDto dto, string? noteText, string userId);
    Task UpdateAsync(MemberDto dto, string? noteText, string userId);
    Task SaveRelationshipsAsync(int memberId, List<RelatedMemberDto> relationships);
    Task DeleteAsync(int id);
}
