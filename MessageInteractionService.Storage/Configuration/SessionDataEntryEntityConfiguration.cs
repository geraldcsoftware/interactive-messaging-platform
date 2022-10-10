using MessageInteractionService.Storage.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageInteractionService.Storage.Configuration;

public class SessionDataEntryEntityConfiguration: IEntityTypeConfiguration<SessionDataEntry>
{
    public void Configure(EntityTypeBuilder<SessionDataEntry> builder)
    {
        builder.ToTable("SessionData");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).IsRequired();
        builder.Property(d => d.SessionId).IsRequired();
        builder.Property(d => d.Key).IsRequired().IsUnicode(false).HasMaxLength(128);
        builder.Property(d => d.Value).IsRequired(false).IsUnicode().HasMaxLength(4000);

        builder.HasOne<Session>()
               .WithMany(s => s.DataEntries)
               .HasForeignKey(d => d.SessionId);
    }
}