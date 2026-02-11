namespace Directory.Application.DTOs;

public class SystemExceptionDto
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Module { get; set; } = string.Empty;
    public string ExceptionMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
}
