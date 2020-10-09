using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniRockz.Threading.Configuration;
using UniRockz.Threading.Core;
using Hangfire.Mongo;
using MongoDB.Driver;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;

namespace UniRockz.Threading.Extensions
{
    public static class ThreadingServiceCollectionExtensions
    {
        public static IServiceCollection AddThreading(this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = new ThreadingSettings();
            configuration.Bind(settings);

            services.Configure<ThreadingSettings>(configuration);
            services.AddSingleton<IBackgroundProcessor, BackgroundProcessor>();
            services.AddHangfire((sp, conf) =>
            {
                conf.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMongoStorage(sp.GetRequiredService<IMongoClient>() as MongoClient,
                        settings.Storage.Database, new MongoStorageOptions()
                        {
                            Prefix = settings.Storage.Prefix,
                            MigrationOptions = new MongoMigrationOptions
                            {
                                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                                BackupStrategy = new CollectionMongoBackupStrategy()
                            },
                            CheckConnection = true
                        });
            });
            services.AddSingleton<IRecurringJobManager, RecurringJobManager>();

            return services;
        }

        public static IServiceCollection AddCronScheduler(this IServiceCollection services)
        {
            return services.AddHostedService<CronScheduler>();
        }

        public static IApplicationBuilder UseThreadingMonitor(this IApplicationBuilder builder)
        {
            builder.UseHangfireServer(
                new BackgroundJobServerOptions
                {
                    WorkerCount = 5
                })
                .UseHangfireDashboard("/background", new DashboardOptions());

            return builder;
        }
    }
}
