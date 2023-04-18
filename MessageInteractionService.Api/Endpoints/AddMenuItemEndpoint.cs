using FastEndpoints;
using MessageInteractionService.Api.Endpoints.Models;
using MessageInteractionService.Storage;
using MessageInteractionService.Storage.DbModels;

namespace MessageInteractionService.Api.Endpoints;

public class AddMenuItemEndpoint : Endpoint<AddMenuItemRequest, MenuItemResponse>
{
    private readonly MessagingDbContext _dbContext;
    private readonly ILogger<AddMenuItemEndpoint> _logger;

    public AddMenuItemEndpoint(MessagingDbContext dbContext,
                               ILogger<AddMenuItemEndpoint> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public override void Configure()
    {
        Post("api/menu-configuration");
        Policies("MenuConfiguration.Write");
    }

    public override async Task HandleAsync(AddMenuItemRequest req, CancellationToken ct)
    {
        _dbContext.MenuDefinitions.Add(new MenuDefinition
        {
            DisplayOrder = req.SortPosition,
            ParentMenuId = req.ParentId,
            DisplayText = req.Title,
            OptionText = req.Title,
            Id = Guid.NewGuid(),
            HandlerName = null
        });
        await _dbContext.SaveChangesAsync(ct);
    }
}