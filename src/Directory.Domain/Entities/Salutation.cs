using Directory.Domain.Interfaces;

namespace Directory.Domain.Entities;

public class Salutation : BaseEntity, ILookupEntity
{
    public string Name { get; set; } = string.Empty;
}
