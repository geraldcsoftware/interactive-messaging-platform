using MessageInteractionService.Core;
using MessageInteractionService.Core.Sessions;
using MessageInteractionService.Storage.DbModels;
using Microsoft.EntityFrameworkCore;

namespace MessageInteractionService.Storage;

public class SessionStore : ISessionStore
{
    private readonly MessagingDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SessionStore(MessagingDbContext dbContext,
                        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ISession?> GetSenderCurrentActiveSession(string sender)
    {
        var sessionQuery = _dbContext.Sessions
                                     .AsNoTracking()
                                     .Include(s => s.Sender)
                                     .Include(s => s.DataEntries)
                                     .Where(s => s.Sender != null && s.Sender.Key == sender && s.Sender.Enabled == true)
                                     .Where(s => s.Terminated == null)
                                     .OrderByDescending(s => s.Start);

        var session = await sessionQuery.FirstOrDefaultAsync();
        if (session == null) return null;

        var dataDictionary = session.DataEntries.ToDictionary(d => d.Key, d => d.Value);
        return new MessagingSession(
                                    session.Id.ToString("N"),
                                    session.Sender!.Key!,
                                    session.Start,
                                    dataDictionary);
    }

    public async Task<ISession> CreateSession(IncomingMessage message)
    {
        var sender = await _dbContext.Senders.FirstOrDefaultAsync(s => s.Key == message.Sender);
        if (sender is { Enabled: false })
            throw new Exception("Sender is not allowed to interact with this service");

        sender ??= new MessageSender
        {
            Created = _dateTimeProvider.UtcNow,
            Enabled = true,
            Id = Guid.NewGuid(),
            Key = message.Sender,
            Updated = null
        };

        var session = new Session
        {
            Id = Guid.NewGuid(),
            Start = _dateTimeProvider.UtcNow,
            Terminated = null!,
            Sender = sender,
            SenderId = sender.Id
        };

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync();

        return new MessagingSession(session.Id.ToString("N"),
                                    sender.Key!,
                                    session.Start,
                                    new Dictionary<string, string>());
    }

    public async Task<ISession> UpdateSession(ISession session)
    {
        var dbSession = await _dbContext.Sessions
                                        .AsNoTracking()
                                        .Include(s => s.Sender)
                                        .Include(s => s.DataEntries)
                                        .FirstOrDefaultAsync(s => s.Id == Guid.Parse(session.Id));
        if (dbSession is null) throw new InvalidOperationException("Session not found in database");

        
        foreach (var (key, value) in session.Data)
        {
            var dataEntry = dbSession.DataEntries
                                     .FirstOrDefault(d => string.Equals(d.Key, key,
                                                                        StringComparison.OrdinalIgnoreCase));
            if (dataEntry is not null)
            {
                if (dataEntry.Value == value) continue;
                dataEntry = dataEntry with { Value = value };
                _dbContext.Update(dataEntry);
            }
            else
            {
                dataEntry = new SessionDataEntry(Guid.NewGuid(), dbSession.Id, key, value);
                dbSession.DataEntries.Add(dataEntry);
                _dbContext.Add(dataEntry);
            }
        }

        await _dbContext.SaveChangesAsync();
        var dataDictionary = dbSession.DataEntries.ToDictionary(d => d.Key, d => d.Value);
        return new MessagingSession(dbSession.Id.ToString("N"),
                                    dbSession.Sender!.Key!,
                                    dbSession.Start,
                                    dataDictionary);
    }

    public async Task TerminateSession(ISession session)
    {
        var updateTime = _dateTimeProvider.UtcNow;
        var rows = await _dbContext.Sessions
                                   .Where(s => s.Id == Guid.Parse(session.Id))
                                   .ExecuteUpdateAsync(update => update.SetProperty(s => s.Terminated, updateTime));

        if (rows != 1) throw new InvalidOperationException("Session not found in database");
    }
}