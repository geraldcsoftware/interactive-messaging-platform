using MessageInteractionService.Core;
using MessageInteractionService.Core.Sessions;
using MessageInteractionService.Storage.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MessageInteractionService.Storage;

public class DbMessageLogger : IMessageLogger
{
    private readonly MessagingDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DbMessageLogger> _logger;

    public DbMessageLogger(MessagingDbContext dbContext,
                           IDateTimeProvider dateTimeProvider,
                           ILogger<DbMessageLogger> logger)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task LogMessage(IncomingMessage message, ISession session)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(session);

        _logger.LogInformation("Logging incoming message {@Message}", message);

        var sender = await _dbContext.Senders.FirstOrDefaultAsync(s => s.Key == session.Sender);
        if (sender == null)
        {
            sender = new MessageSender
            {
                Created = _dateTimeProvider.UtcNow,
                Enabled = true,
                Id = Guid.NewGuid(),
                Key = message.Sender,
                Updated = null
            };
            _dbContext.Senders.Add(sender);
        }

        var messageLog = new IncomingMessageLog
        {
            Body = message.Body,
            From = message.Sender,
            SessionId = Guid.Parse(session.Id),
            Id = Guid.NewGuid(),
            Time = message.ReceivedTime,
            ParticipantId = sender.Id
        };
        _dbContext.MessageLogs.Add(messageLog);

        await _dbContext.SaveChangesAsync();
    }

    public async Task LogMessage(OutgoingMessage message, ISession session)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(session);

        _logger.LogInformation("Logging outgoing message {@Message}", message);

        var sender = await _dbContext.Senders.FirstOrDefaultAsync(s => s.Key == session.Sender);
        if (sender == null)
        {
            sender = new MessageSender
            {
                Created = _dateTimeProvider.UtcNow,
                Enabled = true,
                Id = Guid.NewGuid(),
                Key = message.Recipient,
                Updated = null
            };
            _dbContext.Senders.Add(sender);
        }

        var messageLog = new OutgoingMessageLog
        {
            Body = message.Body,
            To = message.Recipient,
            SessionId = Guid.Parse(session.Id),
            Id = Guid.NewGuid(),
            Time = message.TimeSent,
            ParticipantId = sender.Id
        };
        _dbContext.MessageLogs.Add(messageLog);

        await _dbContext.SaveChangesAsync();
    }
}