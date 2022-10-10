using MessageInteractionService.Storage.DbModels;
using Microsoft.EntityFrameworkCore;

namespace MessageInteractionService.Storage;

public class MessagingDbContext : DbContext
{
    public MessagingDbContext(DbContextOptions<MessagingDbContext> options) : base(options)
    {
    }

    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<MessageSender> Senders => Set<MessageSender>();
    public DbSet<SessionDataEntry> SessionDataEntries => Set<SessionDataEntry>();
        
}