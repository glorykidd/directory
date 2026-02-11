namespace Directory.Domain.Entities;

public class SystemException : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public string Module { get; set; } = string.Empty;
    public string ExceptionMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
}
