namespace Directory.Domain.Entities;

public class SystemSetting : BaseEntity
{
    public string MailServer { get; set; } = string.Empty;
    public int ServerPort { get; set; }
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromUsername { get; set; } = string.Empty;
    public bool RequireAuth { get; set; }
    public bool RequireSsl { get; set; }
}
