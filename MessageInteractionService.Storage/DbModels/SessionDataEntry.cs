namespace MessageInteractionService.Storage.DbModels;

public record SessionDataEntry(
    Guid Id,
    Guid SessionId,
    string Key,
    string Value);
