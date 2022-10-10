namespace MessageInteractionService.Core.Menu;

public interface IMenuProvider
{
    Task<MenuElement> GetRootMenuElement();

    Task<MenuElement?> GetChildMenu(string parentMenuId, int position);
    
}