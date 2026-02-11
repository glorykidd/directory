using Directory.Domain.Interfaces;

namespace Directory.Domain.Entities;

public class RelationshipType : BaseEntity, ILookupEntity
{
    public string Name { get; set; } = string.Empty;
}
