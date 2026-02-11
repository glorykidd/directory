using Directory.Domain.Interfaces;

namespace Directory.Domain.Entities;

public class State : BaseEntity, ILookupEntity
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
}
