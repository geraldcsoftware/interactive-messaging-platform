namespace MessageInteractionService.Core.Menu;

public class MenuElement
{
    public required string HeaderText { get; set; }
    public required string Id { get; set; }
    public required IReadOnlyCollection<MenuOption> Options { get; set; } = new List<MenuOption>();
    public string? HandlerName { get; set; }
}

public class MenuOption
{
    public required string Id { get; set; }
    public required string ParentId { get; set; }
    public required string OptionText { get; set; }
    public required int DisplayPosition { get; set; }
}