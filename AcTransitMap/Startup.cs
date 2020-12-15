using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcTransitMap.Consumers;
using AcTransitMap.Services;
using GreenPipes;
using GtfsConsumer.Entities.Interfaces;
using MassTransit;
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

            services.AddScoped<VehiclePositionConsumer>();

            services.AddMassTransit(cfg =>
            {
                cfg.UsingRabbitMq((context, rabbit) =>
                {
                    //TODO: Make host, user and pass env vars.
                    rabbit.Host("rabbitmq.service", "/", rabbitCfg =>
                    {
                        rabbitCfg.Username("guest");
                        rabbitCfg.Password("guest");
                    });

                    rabbit.ReceiveEndpoint(endpoint =>
                    {
                        endpoint.Bind<IVehiclePosition>();
                        endpoint.Consumer<VehiclePositionConsumer>(context);
                    });
                });
            });

            services.AddMassTransitHostedService();

            services.AddSingleton<IPositionService, PositionService>();

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
            });
        }
    }
}
