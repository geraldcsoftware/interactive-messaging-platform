namespace MessageInteractionService.Core.Fields;

public interface ISessionFieldStore
{
    Task AddFieldRequestToSession(string fieldKey);
    bool HasRequestedField(string fieldKey);
    FieldRawValue? GetRawValue(string fieldKey);
    FieldRequestBuilder FieldRequest(string fieldKey);
    Task UpdateFieldValue(string fieldKey, FieldRawValue rawValue);
    OutgoingMessage Prompt(string promptMessage);
}