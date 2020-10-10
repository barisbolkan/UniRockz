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
using System.Collections.Generic;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UniRockz.Threading.Extensions
{
    public static class ThreadingServiceCollectionExtensions
    {
        private const string HangfirePolicyName = "UniRockz-Asteroids-ThreadingPolicy";

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
            
            services
                .AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => {
                    configuration.Bind("AzureAD", options);
                });
            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.Authority = options.Authority + "/v2.0/";        
                options.TokenValidationParameters.ValidateIssuer = false;
            });
            // Add a new policy for hangfire
            services.AddAuthorization(options =>
            {
                // Policy to be applied to hangfire endpoint
                options.AddPolicy(HangfirePolicyName, builder =>
                {
                    builder
                        .AddAuthenticationSchemes(AzureADDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser();
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
            builder.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 5
            });
            builder.UseHangfireDashboard();
            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard("/background", new DashboardOptions()
                {
                    Authorization = new List<IDashboardAuthorizationFilter> { }
                })
                .RequireAuthorization(HangfirePolicyName);
            });

            return builder;
        }
    }
}
