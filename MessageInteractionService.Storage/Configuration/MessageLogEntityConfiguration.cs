using MessageInteractionService.Storage.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageInteractionService.Storage.Configuration;

public class MessageLogEntityConfiguration : IEntityTypeConfiguration<MessageLog>,
                                             IEntityTypeConfiguration<IncomingMessageLog>,
                                             IEntityTypeConfiguration<OutgoingMessageLog>
{
    public void Configure(EntityTypeBuilder<MessageLog> builder)
    {
        builder.ToTable("MessageLogs");
        builder.HasKey(x => x.Id);

        builder.HasDiscriminator<string>("MessageDirection")
               .HasValue<IncomingMessageLog>("INCOMING")
               .HasValue<OutgoingMessageLog>("OUTGOING");

        builder.Property<string>("MessageDirection").IsRequired().IsUnicode(false).HasMaxLength(20);

        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.SessionId).IsRequired();
        builder.Property(x => x.ParticipantId).IsRequired();
        builder.Property(x => x.Time).IsRequired();
        builder.Property(x => x.Body).IsRequired().IsUnicode().HasMaxLength(4000);
    }

    public void Configure(EntityTypeBuilder<IncomingMessageLog> builder)
    {
        builder.Property(x => x.From).IsRequired().HasMaxLength(200);
    }

    public void Configure(EntityTypeBuilder<OutgoingMessageLog> builder)
    {
        builder.Property(x => x.To).IsRequired().HasMaxLength(200);
    }
}