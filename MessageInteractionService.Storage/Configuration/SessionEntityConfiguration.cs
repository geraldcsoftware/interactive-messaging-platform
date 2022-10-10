using MessageInteractionService.Storage.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageInteractionService.Storage.Configuration;

public class SessionEntityConfiguration: IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).IsRequired();
        builder.Property(s => s.SenderId).IsRequired();
        builder.Property(s => s.Start).IsRequired();
        builder.Property(s => s.Terminated).IsRequired(false);

        builder.HasOne(s => s.Sender)
               .WithMany()
               .HasForeignKey(s => s.SenderId);
    }
}