namespace MessageInteractionService.Api.Endpoints.Models;

public class AddMenuItemRequest
{
    public Guid? ParentId { get; set; }
    public string? Title { get; set; }
    public int SortPosition { get; set; }
}