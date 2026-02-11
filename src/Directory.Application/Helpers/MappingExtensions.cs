using Directory.Application.DTOs;
using Directory.Domain.Entities;

namespace Directory.Application.Helpers;

public static class MappingExtensions
{
    public static MemberListDto ToListDto(this Member member)
    {
        return new MemberListDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            CellPhone = FormatHelpers.FormatPhone(member.CellPhone),
            HomePhone = FormatHelpers.FormatPhone(member.HomePhone),
            LastUpdate = member.ModifiedDate
        };
    }

    public static MemberDetailDto ToDetailDto(this Member member)
    {
        return new MemberDetailDto
        {
            Id = member.Id,
            DisplayName = member.DisplayName,
            MaritalStatus = member.MaritalStatus?.Name ?? string.Empty,
            FormattedAddress = FormatHelpers.FormatAddress(
                member.Address1, member.Address2,
                member.City, member.State?.Name, member.ZipCode),
            CellPhone = FormatHelpers.FormatPhone(member.CellPhone),
            HomePhone = FormatHelpers.FormatPhone(member.HomePhone),
            Email1 = member.Email1,
            Email2 = member.Email2,
            DateOfBirth = member.DateOfBirth?.ToShortDateString() ?? string.Empty,
            MarriageDate = member.MarriageDate?.ToShortDateString() ?? string.Empty,
            RelatedMembers = member.RelationshipsAsSource
                .Select(r => new RelatedMemberDto
                {
                    MemberId = r.RelatedMemberId,
                    DisplayName = r.RelatedMember?.DisplayName ?? string.Empty,
                    RelationshipTypeId = r.RelationshipTypeId,
                    RelationshipTypeName = r.RelationshipType?.Name ?? string.Empty
                })
                .ToList(),
            Notes = member.Notes
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => n.ToNoteDto())
                .ToList()
        };
    }

    public static MemberDto ToEditDto(this Member member)
    {
        return new MemberDto
        {
            Id = member.Id,
            SalutationId = member.SalutationId,
            FirstName = member.FirstName,
            MiddleName = member.MiddleName,
            LastName = member.LastName,
            Suffix = member.Suffix,
            Gender = member.Gender,
            MaritalStatusId = member.MaritalStatusId,
            DateOfBirth = member.DateOfBirth,
            MarriageDate = member.MarriageDate,
            Address1 = member.Address1,
            Address2 = member.Address2,
            City = member.City,
            StateId = member.StateId,
            ZipCode = member.ZipCode,
            CellPhone = FormatHelpers.FormatPhone(member.CellPhone),
            HomePhone = FormatHelpers.FormatPhone(member.HomePhone),
            Email1 = member.Email1,
            Email2 = member.Email2,
            RelatedMembers = member.RelationshipsAsSource
                .Select(r => new RelatedMemberDto
                {
                    MemberId = r.RelatedMemberId,
                    DisplayName = r.RelatedMember?.DisplayName ?? string.Empty,
                    RelationshipTypeId = r.RelationshipTypeId,
                    RelationshipTypeName = r.RelationshipType?.Name ?? string.Empty
                })
                .ToList(),
            Notes = member.Notes
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => n.ToNoteDto())
                .ToList()
        };
    }

    public static void ApplyToEntity(this MemberDto dto, Member entity)
    {
        entity.SalutationId = dto.SalutationId;
        entity.FirstName = dto.FirstName;
        entity.MiddleName = dto.MiddleName;
        entity.LastName = dto.LastName;
        entity.Suffix = dto.Suffix;
        entity.Gender = dto.Gender;
        entity.MaritalStatusId = dto.MaritalStatusId;
        entity.DateOfBirth = dto.DateOfBirth;
        entity.MarriageDate = dto.MarriageDate;
        entity.Address1 = dto.Address1;
        entity.Address2 = dto.Address2;
        entity.City = dto.City;
        entity.StateId = dto.StateId;
        entity.ZipCode = FormatHelpers.CleanZip(dto.ZipCode);
        entity.CellPhone = FormatHelpers.CleanPhone(dto.CellPhone);
        entity.HomePhone = FormatHelpers.CleanPhone(dto.HomePhone);
        entity.Email1 = dto.Email1;
        entity.Email2 = dto.Email2;
        entity.ModifiedDate = DateTime.UtcNow;
    }

    public static MemberNoteDto ToNoteDto(this MemberNote note)
    {
        return new MemberNoteDto
        {
            Id = note.Id,
            NoteText = note.NoteText,
            CreatedDate = note.CreatedDate
        };
    }

    public static SystemSettingDto ToDto(this SystemSetting entity)
    {
        return new SystemSettingDto
        {
            Id = entity.Id,
            MailServer = entity.MailServer,
            ServerPort = entity.ServerPort,
            SmtpUser = entity.SmtpUser,
            SmtpPassword = entity.SmtpPassword,
            FromEmail = entity.FromEmail,
            FromUsername = entity.FromUsername,
            RequireAuth = entity.RequireAuth,
            RequireSsl = entity.RequireSsl
        };
    }

    public static void ApplyToEntity(this SystemSettingDto dto, SystemSetting entity)
    {
        entity.MailServer = dto.MailServer;
        entity.ServerPort = dto.ServerPort;
        entity.SmtpUser = dto.SmtpUser;
        entity.SmtpPassword = dto.SmtpPassword;
        entity.FromEmail = dto.FromEmail;
        entity.FromUsername = dto.FromUsername;
        entity.RequireAuth = dto.RequireAuth;
        entity.RequireSsl = dto.RequireSsl;
    }

    public static SystemExceptionDto ToDto(this Domain.Entities.SystemException entity)
    {
        return new SystemExceptionDto
        {
            Id = entity.Id,
            Timestamp = entity.Timestamp,
            Module = entity.Module,
            ExceptionMessage = entity.ExceptionMessage,
            StackTrace = entity.StackTrace
        };
    }
}
