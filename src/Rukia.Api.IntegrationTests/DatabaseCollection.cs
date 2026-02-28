using Xunit;

namespace Rukia.Api.IntegrationTests;

[CollectionDefinition("BancoDeTeste")]
public sealed class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}