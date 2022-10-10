﻿using MessageInteractionService.Core;
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
                                     .Include(s => s.DataEntries)
                                     .Where(s => s.Sender != null && s.Sender.Key == sender && s.Sender.Enabled == true)
                                     .Where(s => s.Terminated == null)
                                     .OrderByDescending(s => s.Start);

        var session = await sessionQuery.FirstOrDefaultAsync();
        if (session == null) return null;

        var dataDictionary = session.DataEntries.ToDictionary(d => d.Key!, d => d.Value!);
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

        var session = new DbModels.Session
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
                                        .Include(s => s.DataEntries)
                                        .FirstOrDefaultAsync(s => s.Id == Guid.Parse(session.Id));
        if (dbSession is null) throw new InvalidOperationException("Session not found in database");

        foreach (var (key, value) in session.Data)
        {
            var dataEntry = dbSession.DataEntries
                                     .FirstOrDefault(d => string.Equals(d.Key, key,
                                                                        StringComparison.OrdinalIgnoreCase));
            if (dataEntry is not null) dataEntry.Value = value;
            else
            {
                dataEntry = new SessionDataEntry
                {
                    Key = key,
                    Value = value,
                    SessionId = dbSession.Id,
                };
                dbSession.DataEntries.Add(dataEntry);
            }
        }

        await _dbContext.SaveChangesAsync();
        var dataDictionary = dbSession.DataEntries.ToDictionary(d => d.Key!, d => d.Value!);
        return new MessagingSession(
                                    dbSession.Id.ToString("N"),
                                    dbSession.Sender!.Key!,
                                    dbSession.Start,
                                    dataDictionary);
    }
}