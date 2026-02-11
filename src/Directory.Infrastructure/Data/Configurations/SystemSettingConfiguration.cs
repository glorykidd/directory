using Directory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Directory.Infrastructure.Data.Configurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.MailServer).HasMaxLength(200);
        builder.Property(s => s.SmtpUser).HasMaxLength(200);
        builder.Property(s => s.SmtpPassword).HasMaxLength(500);
        builder.Property(s => s.FromEmail).HasMaxLength(200);
        builder.Property(s => s.FromUsername).HasMaxLength(200);
    }
}

public class SystemExceptionConfiguration : IEntityTypeConfiguration<Domain.Entities.SystemException>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.SystemException> builder)
    {
        builder.ToTable("SystemExceptions");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Module).HasMaxLength(200);
        builder.Property(e => e.ExceptionMessage).IsRequired();
    }
}
