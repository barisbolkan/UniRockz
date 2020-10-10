using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using UniRockz.Domain.Asteroids.Commands;
using UniRockz.Domain.Asteroids.Events;
using UniRockz.Domain.Utils;
using UniRockz.Messaging.Handlers;
using UniRockz.Repository.Asteroids;
using UniRockz.Repository.Asteroids.Entities;

namespace UniRockz.Domain.Asteroids.Handlers
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public Uri Uri { get; set; }

        public HttpResponseException(HttpStatusCode status, Uri uri, string message)
            : base(message)
        {
            StatusCode = status;
            Uri = uri;
        }
    }

    public class EmptyDataException : Exception
    {
        public EmptyDataException(string message)
            : base(message)
        { }
    }

    public class RetrieveAsteroidsHandler : IRequestHandler<RetrieveAsteroidsCommand, Unit>
    {
        private readonly IAsteroidRepository _repository;
        private readonly HttpClient _client;
        private readonly ILogger<RetrieveAsteroidsHandler> _logger;

        public RetrieveAsteroidsHandler(IAsteroidRepository repository,
            HttpClient httpClient, ILogger<RetrieveAsteroidsHandler> logger)
        {
            _repository = repository;
            _client = httpClient;
            _logger = logger;
        }

        public async Task<Unit> Handle(RetrieveAsteroidsCommand request, CancellationToken cancellationToken)
        {
            // Paginate the API
            var url = new Uri($"{request.BaseUrl}{request.StartPage}");
            IEnumerable<AsteroidInfo> data = null;

            do
            {
                (url, data) = await _client.GetAsync(url)
                    .FlatMap(resp =>
                    {
                        return resp.StatusCode switch
                        {
                            HttpStatusCode.OK => ToObjectAsync(resp.Content),
                            _ => throw new HttpResponseException(resp.StatusCode, url,
                            $"The response retrieved from Url[{url}] has errors.")
                        };
                    });

                if (data != null)
                {
                    // Persist the data
                    await _repository.BulkUpsert(data);
                }
                else
                {
                    _logger.LogWarning($"The requested url[{url}] returned null data!");
                }
            } while (url != null);

            return Unit.Value;
        }

        private async Task<(Uri, IEnumerable<AsteroidInfo>)> ToObjectAsync(HttpContent content)
        {
            using var stream = await content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);
            string next = null;
            IEnumerable<AsteroidInfo> data = null;

            if (document.RootElement.TryGetProperty("links", out var links))
            {
                next = links.EnumerateObject()
                    .Where(f => f.Name == "next")
                    .Select(f => f.Value.GetString())
                    .FirstOrDefault();
            }

            if (document.RootElement.TryGetProperty("near_earth_objects", out var neo))
            {
                data = neo.EnumerateArray()
                    .Select(json =>
                    {
                        IList<Diameter> ParseDiameter()
                        {
                            if (json.TryGetProperty("estimated_diameter", out var diameter))
                            {
                                return diameter.EnumerateObject()
                                    .Select(d =>
                                        new Diameter()
                                        {
                                            Min = d.Value.GetProperty("estimated_diameter_min").GetDouble(),
                                            Max = d.Value.GetProperty("estimated_diameter_max").GetDouble(),
                                            Unit = Enum.Parse<DiameterUnits>(d.Name, true)
                                        }).ToList();
                            }
                            return null;
                        }

                        List<ApproachData> ParseApproachData()
                        {
                            return json.GetProperty("close_approach_data")
                                .EnumerateArray()
                                .Select(d => new ApproachData()
                                {
                                    Date = DateTimeOffset.FromUnixTimeMilliseconds(d.GetProperty<long>("epoch_date_close_approach").Value),
                                    MissDistances = d.GetProperty("miss_distance")
                                        .EnumerateObject()
                                        .Select(md => new Distance()
                                        {
                                            Value = md.GetValue<Double>(),
                                            Unit = Enum.Parse<DistanceUnits>(md.Name, true)
                                        }).ToList(),
                                    OrbitingBody = d.GetProperty("orbiting_body").GetString(),
                                    RelativeVelocities = d.GetProperty("relative_velocity")
                                        .EnumerateObject()
                                        .Select(v => new Velocity()
                                        {
                                            Value = v.GetValue<Double>(),
                                            Unit = Enum.Parse<VelocityUnits>(v.Name.Replace("_", ""), true)
                                        }).ToList()
                                }).ToList();
                        }

                        if (json.TryGetProperty("id", out var asteroidId) &&
                            json.TryGetProperty("neo_reference_id", out var neoRefId) &&
                            json.TryGetProperty("name", out var name))
                        {
                            return new AsteroidInfo(asteroidId.GetString(), neoRefId.GetString(), name.GetString())
                            {
                                AbsoluteMagnitude = json.GetProperty<Double>("absolute_magnitude_h"),
                                CloseApproachData = ParseApproachData(),
                                EstimatedDiameters = ParseDiameter(),
                                IsHazardous = json.GetProperty<Boolean>("is_potentially_hazardous_asteroid"),
                                JplUrl = json.GetProperty("nasa_jpl_url").GetString()
                            };
                        }

                        return null;

                    }).Where(_ => _ != null)
                    .ToList();
            }

            return (string.IsNullOrEmpty(next) ? null : new Uri(next), data);
        }
    }
}
