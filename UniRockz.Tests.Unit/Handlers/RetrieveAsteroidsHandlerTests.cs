using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq;
using Moq.Language.Flow;
using Moq.Protected;
using UniRockz.Domain.Asteroids.Commands;
using UniRockz.Domain.Asteroids.Events;
using UniRockz.Domain.Asteroids.Handlers;
using UniRockz.Messaging.Handlers;
using UniRockz.Repository.Asteroids;
using UniRockz.Repository.Asteroids.Entities;
using UniRockz.Repository.Core;
using UniRockz.Tests.Unit.Fixtures;
using Xunit;

namespace UniRockz.Tests.Unit.Handlers
{
    public class RetrieveAsteroidsHandlerTests
    {
        public RetrieveAsteroidsHandlerTests()
        { }

        [Fact(DisplayName = "RetrieveAsteroidsHandler should fail with UriFormatException for invalid uri")]
        public async Task RetrieveAsteroidsHandlerShouldThrowUriFormatException()
        {
            var repository = new Mock<IAsteroidRepository>();
            var logger = new Mock<ILogger<RetrieveAsteroidsHandler>>();
            var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
                .Verifiable();
            var publisher = new Mock<IEventPublisher<AsteroidRetrievedEvent>>();

            var command = new RetrieveAsteroidsCommand("invalid base url", "invalid start page url");
            var handler = new RetrieveAsteroidsHandler(repository.Object, new HttpClient(httpMock.Object), logger.Object);

            await Assert.ThrowsAsync<UriFormatException>(() =>
                handler.Handle(command, new System.Threading.CancellationToken())
            );
        }

        [Fact(DisplayName = "RetrieveAsteroidsHandler should fail with HttpResponseException for not OK status code")]
        public async Task RetrieveAsteroidsHandlerShouldThrowHttpResponseException()
        {
            var repository = new Mock<IAsteroidRepository>();
            var logger = new Mock<ILogger<RetrieveAsteroidsHandler>>();
            var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError))
                .Verifiable();
            var publisher = new Mock<IEventPublisher<AsteroidRetrievedEvent>>();

            var baseUrl = "http://www.google.com/";
            var startPage = string.Empty;
            var command = new RetrieveAsteroidsCommand(baseUrl, startPage);
            var handler = new RetrieveAsteroidsHandler(repository.Object, new HttpClient(httpMock.Object), logger.Object);

            var ex = await Assert.ThrowsAsync<HttpResponseException>(() =>
                handler.Handle(command, new System.Threading.CancellationToken())
            );

            Assert.Equal<int>(0,
                    Uri.Compare(new Uri($"{baseUrl}{startPage}"),
                        ex.Uri, UriComponents.HttpRequestUrl, UriFormat.UriEscaped, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        [Fact(DisplayName = "RetrieveAsteroidsHandler should fail with JsonException for non json data")]
        public async Task RetrieveAsteroidsHandlerShouldThrowJsonReaderException()
        {
            var repository = new Mock<IAsteroidRepository>();
            var logger = new Mock<ILogger<RetrieveAsteroidsHandler>>();
            var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("<xml>")
                })
                .Verifiable();
            var publisher = new Mock<IEventPublisher<AsteroidRetrievedEvent>>();

            var baseUrl = "http://www.google.com/";
            var startPage = string.Empty;
            var command = new RetrieveAsteroidsCommand(baseUrl, startPage);
            var handler = new RetrieveAsteroidsHandler(repository.Object, new HttpClient(httpMock.Object), logger.Object);

            await Assert.ThrowsAnyAsync<JsonException>(() =>
                handler.Handle(command, new System.Threading.CancellationToken())
            );
        }

        [Fact(DisplayName = "RetrieveAsteroidsHandler should not fail if the json data does not have 'links' field")]
        public async Task RetrieveAsteroidsHandlerShouldNotThrowExceptionIfLinksIsMissing()
        {
            var repository = new Mock<IAsteroidRepository>();
            var logger = new Mock<ILogger<RetrieveAsteroidsHandler>>();
            var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                })
                .Verifiable();
            var publisher = new Mock<IEventPublisher<AsteroidRetrievedEvent>>();

            var baseUrl = "http://www.google.com/";
            var startPage = string.Empty;
            var command = new RetrieveAsteroidsCommand(baseUrl, startPage);
            var handler = new RetrieveAsteroidsHandler(repository.Object, new HttpClient(httpMock.Object), logger.Object);

            await handler.Handle(command, new System.Threading.CancellationToken());
        }

        [Fact(DisplayName = "RetrieveAsteroidsHandler should not fail if the json data does not have 'near_earth_objects' field")]
        public async Task RetrieveAsteroidsHandlerShouldNotThrowExceptionIfNearEarthObjectsIsMissing()
        {
            var repository = new Mock<IAsteroidRepository>();
            var logger = new Mock<ILogger<RetrieveAsteroidsHandler>>();
            var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"links\":{ \"next\": \"http://www.google.com/\" }}")
                })
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });
            var publisher = new Mock<IEventPublisher<AsteroidRetrievedEvent>>();

            var baseUrl = "http://www.google.com/";
            var startPage = string.Empty;
            var command = new RetrieveAsteroidsCommand(baseUrl, startPage);
            var handler = new RetrieveAsteroidsHandler(repository.Object, new HttpClient(httpMock.Object), logger.Object);

            repository.Verify(r => r.BulkUpsert(It.IsAny<IEnumerable<AsteroidInfo>>()), Times.Never());
            await handler.Handle(command, new System.Threading.CancellationToken());
        }
    }
}
