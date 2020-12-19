using AcTransitMap.Database;
using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MessageProcessor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            LoggerConfiguration config = new LoggerConfiguration()
                .WriteTo.Console();
            string seqApiKey = Environment.GetEnvironmentVariable("SEQ_API_KEY");
            string seqUrl = Environment.GetEnvironmentVariable("SEQ_URL");
            if (!string.IsNullOrEmpty(seqApiKey) && !string.IsNullOrEmpty(seqUrl))
                config.WriteTo.Seq(seqUrl, apiKey: seqApiKey);

            Log.Logger = config
                .MinimumLevel.Information()
                .CreateLogger();

            IServiceCollection services = new ServiceCollection();

            ConfigureServices(services);

            IServiceProvider provider = services.BuildServiceProvider();

            Log.Information("Started MessageProcessor.");
            try
            {
                await provider.GetRequiredService<IBusControl>().StartAsync();
                Console.Read();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"A(n) {ExceptionType} occured while starting the message bus: {ExceptionMessage}.", ex.GetType().Name, ex.Message);
            }
            finally
            {
                await provider.GetRequiredService<IBusControl>().StopAsync();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            string rabbitUser = Environment.GetEnvironmentVariable("RABBIT_USER");
            string rabbitPass = Environment.GetEnvironmentVariable("RABBIT_PASS");

            if (string.IsNullOrEmpty(rabbitUser) || string.IsNullOrEmpty(rabbitPass))
            {
                Log.Fatal("RabbitMQ User/Pass cannot be null/undefined.");
                throw new ArgumentNullException("RabbitMQ User & Pass cannot be null.");
            }

            string mongoUrl = Environment.GetEnvironmentVariable("MONGO_CONNSTR");
            string mongoDb = Environment.GetEnvironmentVariable("MONGO_DB");
            string mongoColl = Environment.GetEnvironmentVariable("MONGO_COLLECTION");

            services.AddSingleton<IDbConnector<UpdatedVehiclePosition, string>, MongoDbConnector>(x => new MongoDbConnector(mongoUrl,mongoDb,mongoColl));
            services.AddScoped<IUpdaterService, UpdaterService>();
            services.AddScoped<VehiclePositionConsumer>(); // Necessary to prevent MT DI faults.
            services.AddMassTransit(mt =>
            {
                mt.UsingRabbitMq((context, rabbit) => {
                    rabbit.Host("rabbitmq.service", "/", config =>
                    {
                        config.Username(rabbitUser);
                        config.Password(rabbitPass);
                    });
                    rabbit.ReceiveEndpoint(endpoint =>
                    {
                        endpoint.Bind<IVehiclePosition>();
                        endpoint.Consumer<VehiclePositionConsumer>(context);
                    });
                });
            });
        }
    }
}
