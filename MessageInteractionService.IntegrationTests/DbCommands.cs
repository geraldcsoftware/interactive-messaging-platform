namespace MessageInteractionService.IntegrationTests;

public static class DbCommands
{
    public const string InsertMenuCommand = """
        INSERT INTO "MenuDefinitions" ("Id", "ParentMenuId", "DisplayText", "OptionText", "DisplayOrder", "HandlerName")
        VALUES (@Id, @ParentMenuId, @DisplayText, @OptionText, @DisplayOrder, @HandlerName)
        """;
}