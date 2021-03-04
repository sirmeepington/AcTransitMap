using AcTransitMap.Database;
using AcTransitMap.Shared.Entities;
using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageProcessor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            InitLogging();

            IServiceCollection services = new ServiceCollection();

            ConfigureServices(services);

            IServiceProvider provider = services.BuildServiceProvider();

            Log.Information("Started MessageProcessor.");
            try
            {
                await provider.GetRequiredService<IBusControl>().StartAsync();
                await Task.Delay(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "A(n) {ExceptionType} occured while starting the message bus: {ExceptionMessage}.", ex.GetType().Name, ex.Message);
            }
            finally
            {
                await provider.GetRequiredService<IBusControl>().StopAsync();
                Log.Information("Closing MessageProcessor");
            }
        }

        private static void InitLogging()
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
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            RabbitConnection rabbitConnection = new RabbitConnection()
            {
                Endpoint = Environment.GetEnvironmentVariable("RABBIT_URL"),
                Password = Environment.GetEnvironmentVariable("RABBIT_PASS"),
                Username = Environment.GetEnvironmentVariable("RABBIT_USER")
            };

            if (!rabbitConnection.IsValid())
            {
                Log.Fatal("RabbitMQ User/Pass cannot be null/undefined.");
                throw new ArgumentNullException("RabbitMQ connection details are invalid.");
            }

            string mongoUrl = Environment.GetEnvironmentVariable("MONGO_CONNSTR");
            string mongoDb = Environment.GetEnvironmentVariable("MONGO_DB");
            string mongoColl = Environment.GetEnvironmentVariable("MONGO_COLLECTION");

            services.AddSingleton<IDbConnector<UpdatedVehiclePosition, string>, MongoDbConnector>(x => new MongoDbConnector(mongoUrl,mongoDb,mongoColl));
            services.AddScoped<IUpdaterService, UpdaterService>();
            services.AddScoped<VehiclePositionConsumer>(); // Necessary to prevent MT DI faults.
            services.AddMassTransit(mt =>
            {
                mt.UsingRabbitMq((context, rabbit) => ConfigureRabbit(context, rabbit, rabbitConnection));
            });
        }

        /// <summary>
        /// Configures the RabbitMQ message broker with settings specified by the 
        /// <paramref name="connection"/> object
        /// </summary>
        /// <param name="context">The <see cref="IBusRegistrationContext"/> for the bus</param>
        /// <param name="rabbit">The <see cref="IRabbitMqBusFactoryConfigurator"/> configuration</param>
        /// <param name="connection">The <see cref="RabbitConnection"/> details.</param>
        private static void ConfigureRabbit(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator rabbit, RabbitConnection connection)
        {
            rabbit.Host(connection);
            rabbit.ReceiveEndpoint(endpoint =>
            {
                endpoint.Bind<IVehiclePosition>();
                endpoint.Consumer<VehiclePositionConsumer>(context);
            });
        }
    }
}
