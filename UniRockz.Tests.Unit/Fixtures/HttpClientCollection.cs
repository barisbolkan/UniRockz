using Xunit;

namespace UniRockz.Tests.Unit.Fixtures
{
    [CollectionDefinition("HttpClient Collection")]
    public class HttpClientCollection : ICollectionFixture<HttpClientFixture>
    { }
}
