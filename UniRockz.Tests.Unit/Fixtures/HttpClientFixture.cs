using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;
using Moq.Protected;

namespace UniRockz.Tests.Unit.Fixtures
{
    public class HttpClientFixture : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _mockObject;

        public HttpClientFixture()
        {
            _mockObject = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        }

        public HttpClient GetHttpClient(HttpResponseMessage response)
        {
            _mockObject
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response)
                .Verifiable();

            return new HttpClient(_mockObject.Object);
        }

        public void Dispose()
        { }
    }
}
