namespace MessageInteractionService.IntegrationTests;

[CollectionDefinition("SharedMenu")]
public class SharedMenuCollectionDefinition : ICollectionFixture<SingleMenuItemDataFixture>
{
}

[CollectionDefinition("ExtendedMenuData")]
public class ExtendedMenuDataCollectionDefinition : ICollectionFixture<ExtendedMenuTreeDataFixture>
{
}