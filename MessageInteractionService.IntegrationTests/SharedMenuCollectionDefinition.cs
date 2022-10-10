namespace MessageInteractionService.IntegrationTests;

[CollectionDefinition("SharedMenu")]

public class SharedMenuCollectionDefinition:  ICollectionFixture<DataSetupFixture>,
                                              ICollectionFixture<ApplicationFactory>
{
}