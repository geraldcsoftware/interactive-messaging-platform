using MessageInteractionService.Storage.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageInteractionService.Storage.Configuration;

public class MessageSenderEntityConfiguration: IEntityTypeConfiguration<MessageSender>
{
    public void Configure(EntityTypeBuilder<MessageSender> builder)
    {
        builder.ToTable("Senders");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).IsRequired();
        builder.Property(s => s.Key).IsRequired().IsUnicode(false).HasMaxLength(128);
        builder.Property(s => s.Enabled).IsRequired();
        builder.Property(s => s.Created).IsRequired();
        builder.Property(s => s.Updated).IsRequired(false);
    }
}