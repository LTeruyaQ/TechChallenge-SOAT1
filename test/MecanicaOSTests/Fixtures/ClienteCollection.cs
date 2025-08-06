using Xunit;

namespace MecanicaOSTests.Fixtures
{
    [CollectionDefinition(nameof(ClienteCollection))]
    public class ClienteCollection : ICollectionFixture<ClienteFixture>
    {
    }
}
