using System.Text.Json;

namespace MessageInteractionService.Core.Fields;

public class SessionFieldStore : ISessionFieldStore
{
    private readonly ISession _session;
    private readonly ISessionStore _sessionStore;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IReadOnlyCollection<string> _requestedFields;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public SessionFieldStore(ISession session,
                             ISessionStore sessionStore,
                             IDateTimeProvider dateTimeProvider)
    {
        _session = session;
        _sessionStore = sessionStore;
        _dateTimeProvider = dateTimeProvider;

        var hasFieldRequests = session.Data.TryGetValue("FieldRequests", out var requestedFields);
        _requestedFields = hasFieldRequests && !string.IsNullOrEmpty(requestedFields)
                               ? requestedFields.Split(',')
                               : ArraySegment<string>.Empty;
    }

    public async Task AddFieldRequestToSession(string fieldKey)
    {
        var fields = new HashSet<string>(_requestedFields) { fieldKey };

        _session.Data["FieldRequests"] = string.Join(',', fields);
        await _sessionStore.UpdateSession(_session);
    }

    public bool HasRequestedField(string fieldKey)
    {
        return _requestedFields.Contains(fieldKey);
    }

    public FieldRawValue? GetRawValue(string fieldKey)
    {
        return _session.Data.TryGetValue(fieldKey, out var value)
                   ? JsonSerializer.Deserialize<FieldRawValue>(value, _jsonSerializerOptions)
                   : null;
    }

    public FieldRequestBuilder FieldRequest(string fieldKey)
    {
        return FieldRequestBuilder.New(this, fieldKey);
    }

    public async Task UpdateFieldValue(string fieldKey, FieldRawValue rawValue)
    {
        var storeValue = JsonSerializer.Serialize(rawValue, _jsonSerializerOptions);
        _session.Data[fieldKey] = storeValue;
        await _sessionStore.UpdateSession(_session);
    }

    public OutgoingMessage Prompt(string promptMessage)
    {
        return new()
        {
            Body = promptMessage,
            Recipient = _session.Sender,
            SessionId = _session.Id,
            TimeSent = _dateTimeProvider.UtcNow,
            TerminateSession = false
        };
    }
}