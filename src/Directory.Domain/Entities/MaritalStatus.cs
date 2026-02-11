using Directory.Domain.Interfaces;

namespace Directory.Domain.Entities;

public class MaritalStatus : BaseEntity, ILookupEntity
{
    public string Name { get; set; } = string.Empty;
}
