namespace MessageInteractionService.Storage.DbModels;

public class MenuDefinition
{
    public Guid Id { get; set; }
    public string? DisplayText { get; set; }
    public string? OptionText { get; set; }
    public int DisplayOrder { get; set; }
    public Guid? ParentMenuId { get; set; }
    public string? HandlerName { get; set; }
}