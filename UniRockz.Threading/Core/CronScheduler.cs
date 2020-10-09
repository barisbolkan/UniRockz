using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UniRockz.Threading.Core
{
    public class CronScheduler : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundProcessor _backgroundProcessor;

        public CronScheduler(IServiceProvider serviceProvider,
            IBackgroundProcessor backgroundProcessor)
        {
            _serviceProvider = serviceProvider;
            _backgroundProcessor = backgroundProcessor;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var jobs = scope.ServiceProvider.GetServices<ICronJob>();

            foreach (var job in jobs)
            {
                _backgroundProcessor.AddCronJob(job);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }
    }
}
