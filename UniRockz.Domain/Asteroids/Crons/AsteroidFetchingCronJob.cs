using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UniRockz.Domain.Asteroids.Commands;
using UniRockz.Threading.Core;

namespace UniRockz.Domain.Asteroids.Crons
{
    public class AsteroidFetchingCronJob : ICronJob
    {
        private readonly IMediator _mediator;
        private readonly CronSettings _settings;

        public AsteroidFetchingCronJob(IMediator mediator, IOptions<CronSettings> options)
        {
            _mediator = mediator;
            _settings = options.Value;
        }

        public string Name => "AsteroidFetcher";

        public string Schedule => _settings.AsteroidFetcher.Schedule;

        public bool Enabled => _settings.AsteroidFetcher.Enabled;

        public async Task Execute(IJobCancellationToken cancellationToken) =>
            await _mediator.Send(new RetrieveAsteroidsCommand(_settings.AsteroidFetcher.BaseUrl, _settings.AsteroidFetcher.StartPage));
    }
}
