namespace MessageInteractionService.Core.Menu;

public class MenuElement
{
    public string HeaderText { get; set; }
    public string Id { get; set; }
    public IReadOnlyCollection<MenuOption> Options { get; set; }
}

public class MenuOption
{
    public string Id { get; set; }
    public string ParentId { get; set; }
    public string OptionText { get; set; }
    public int DisplayPosition { get; set; }
}