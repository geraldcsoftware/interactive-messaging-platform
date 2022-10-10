using MessageInteractionService.Core.Menu;
using Microsoft.EntityFrameworkCore;

namespace MessageInteractionService.Storage;

public class DbMenuProvider : IMenuProvider
{
    private readonly MessagingDbContext _dbContext;

    public DbMenuProvider(MessagingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MenuElement> GetRootMenuElement()
    {
        var menuDefinition = await _dbContext.MenuDefinitions
                                             .Where(m => m.ParentMenuId == null)
                                             .Where(m => m.DisplayOrder == -1)
                                             .OrderBy(m => m.DisplayOrder)
                                             .FirstOrDefaultAsync();

        if (menuDefinition == null)
            throw new Exception("No root menu defined in application");

        var menu = new MenuElement
        {
            Id = menuDefinition.Id.ToString("N"),
            HeaderText = menuDefinition.DisplayText ?? string.Empty,
            Options = await GetSubOptions(menuDefinition.Id),
            HandlerName = menuDefinition.HandlerName
        };
        return menu;
    }

    public async Task<MenuElement?> GetChildMenu(string parentMenuId, int position)
    {
        var parentId = Guid.Parse(parentMenuId);
        var rootMenuDefinition = await _dbContext.MenuDefinitions
                                                 .Where(m => m.ParentMenuId == parentId)
                                                 .Where(m => m.DisplayOrder == position)
                                                 .FirstOrDefaultAsync();

        if (rootMenuDefinition == null)
            return null;

        var menu = new MenuElement
        {
            Id = rootMenuDefinition.Id.ToString("N"),
            HeaderText = rootMenuDefinition.DisplayText ?? string.Empty,
            Options = await GetSubOptions(rootMenuDefinition.Id)
        };
        return menu;
    }

    private async Task<IReadOnlyCollection<MenuOption>> GetSubOptions(Guid parentMenuId)
    {
        var options = await _dbContext.MenuDefinitions.Where(m => m.ParentMenuId == parentMenuId)
                                      .OrderBy(m => m.DisplayOrder)
                                      .Select(m => new MenuOption
                                       {
                                           DisplayPosition = m.DisplayOrder,
                                           Id = m.Id.ToString(),
                                           OptionText = m.OptionText ?? "",
                                           ParentId = parentMenuId.ToString()
                                       })
                                      .ToListAsync();
        return options;
    }
}