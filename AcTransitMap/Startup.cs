using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcTransitMap.Consumers;
using AcTransitMap.Database;
using AcTransitMap.Hubs;
using AcTransitMap.Services;
using GreenPipes;
using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AcTransitMap
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHostedService<PositionUpdaterService>();

            services.AddScoped<VehiclePositionConsumer>();

            string rabbitUser = Environment.GetEnvironmentVariable("RABBIT_USER");
            string rabbitPass = Environment.GetEnvironmentVariable("RABBIT_PASS");

            services.AddMassTransit(cfg =>
            {
                cfg.UsingRabbitMq((context, rabbit) => ConfigureRabbit(context, rabbit, rabbitUser, rabbitPass));
            });
            services.AddMassTransitHostedService();

            string mongoUrl = Environment.GetEnvironmentVariable("MONGO_CONNSTR");
            string mongoDb = Environment.GetEnvironmentVariable("MONGO_DB");
            string mongoColl = Environment.GetEnvironmentVariable("MONGO_COLLECTION");

            services.AddSingleton<IDbConnector<UpdatedVehiclePosition, string>, MongoDbConnector>(x => new MongoDbConnector(mongoUrl,mongoDb,mongoColl));
            services.AddSingleton<IPositionService, PositionService>();

            services.AddSignalR();
        }

        private static void ConfigureRabbit(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator rabbit, string rabbitUser, string rabbitPass)
        {
            rabbit.Host(Environment.GetEnvironmentVariable("RABBIT_URL"), "/", rabbitCfg =>
            {
                rabbitCfg.Username(rabbitUser);
                rabbitCfg.Password(rabbitPass);
            });

            rabbit.ReceiveEndpoint(endpoint =>
            {
                endpoint.Bind<IUpdatedVehiclePosition>();
                endpoint.Consumer<VehiclePositionConsumer>(context);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<MapPositionHub>("mapHub");
            });
        }
    }
}
