using Directory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Directory.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Member> Members { get; }
    DbSet<MemberRelationship> MemberRelationships { get; }
    DbSet<MemberNote> MemberNotes { get; }
    DbSet<State> States { get; }
    DbSet<Salutation> Salutations { get; }
    DbSet<MaritalStatus> MaritalStatuses { get; }
    DbSet<RelationshipType> RelationshipTypes { get; }
    DbSet<SystemSetting> SystemSettings { get; }
    DbSet<Domain.Entities.SystemException> SystemExceptions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
