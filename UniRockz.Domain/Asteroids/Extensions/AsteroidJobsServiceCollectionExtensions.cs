using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniRockz.Domain.Asteroids.Crons;
using UniRockz.Threading.Core;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;

namespace UniRockz.Domain.Asteroids.Extensions
{
    public static class AsteroidJobsServiceCollectionExtensions
    {
        public static IServiceCollection AddCronJobs(this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = new CronSettings();
            configuration.Bind(settings);

            services.Configure<CronSettings>(configuration);
            services.AddSingleton<HttpClient>();
            services.AddTransient<ICronJob, AsteroidFetchingCronJob>();
            services.AddAutoMapper(conf => conf.AddExpressionMapping(), typeof(CronSettings).Assembly);

            return services;
        }
    }
}
