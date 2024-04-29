namespace ChargePlanner.Api.Tests;

[CollectionDefinition(Name)]
public class ComponentTestsCollection : ICollectionFixture<WebAppFactory>
{
    public const string Name = "component tests";
}