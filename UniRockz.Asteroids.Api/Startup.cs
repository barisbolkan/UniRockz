using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UniRockz.Domain.Asteroids.Handlers;
using UniRockz.Repository.Configuration;
using UniRockz.Domain.Asteroids.Extensions;
using Microsoft.AspNet.OData.Extensions;
using UniRockz.Threading.Extensions;
using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using UniRockz.Domain.Asteroids.Responses;
using System;
using UniRockz.Repository.Asteroids.Entities;

namespace UniRockz.Asteroids.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Register Database services
            services
                .AddDatabase(_configuration.GetConnectionString("Mongo")) // Register Database services
                .AddMediatR(typeof(GetAsteroidHandler).Assembly) // Register MediatR
                .AddThreading(_configuration.GetSection("Threading")) // Register Hangfire stuff for background processing
                .AddCronScheduler() // Register cron scheduler for scheduled jobs
                .AddCronJobs(_configuration.GetSection("Crons"));

            //services.AddControllers();

            services.AddMvc(option => option.EnableEndpointRouting = false);
                //.AddNewtonsoftJson();
            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseThreadingMonitor();

            /*app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Item}/{action=Index}/{id?}");

                routes.EnableDependencyInjection();
                routes.Select().Filter().OrderBy().Expand().Count();
            });*/
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Select().Filter().Count().MaxTop(null);
                //routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel(app.ApplicationServices));
                routeBuilder.EnableDependencyInjection();
            });
        }

        IEdmModel GetEdmModel(IServiceProvider app)
        {
            var odataBuilder = new ODataConventionModelBuilder(app);
            var entitySet = odataBuilder.EntitySet<AsteroidInfo>("Asteroids");
            entitySet.EntityType.HasKey(entity => entity.Name);

            return odataBuilder.GetEdmModel();
        }
    }
}
