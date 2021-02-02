using GtfsConsumer.Services;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GtfsConsumer
{
    /// <summary>
    /// This app consumes the GTFS RT feed from AC Transit and publishes it
    /// to the message queue.
    /// <para/>
    /// In a sense it is both a consumer and a producer.
    /// </summary>
    public class ConsumerApp
    {

        static async Task Main(string[] args)
        {
            // Logging
            InitLogging();

            IConsumerBus consumer = await CreateAndStartBus();
            IConsumerService service = CreateService(consumer);

            try
            {
                // Call every 30s
                Timer timer = new Timer(new TimerCallback(service.Publish), null, 0, 30000);
                await Task.Delay(Timeout.Infinite);
            } catch (Exception ex)
            {
                Log.Error(ex, "Exception occured while publishing GTFS-RT feed: {ExceptionMessage}.", ex.Message);
            }
            finally
            {
                Log.Information("Closing the Gtfs Consumer");
                await consumer.Stop();
            }
        }

        /// <summary>
        /// Creates a <see cref="ConsumerService"/> from the given
        /// <see cref="IConsumerBus"/>.
        /// </summary>
        /// <param name="consumer">The consumer bus to create from.</param>
        private static IConsumerService CreateService(IConsumerBus consumer)
        {
            string apiKey = Environment.GetEnvironmentVariable("ACTRANSIT_KEY");
            ITransitConsumer acTransit = null;
            try
            {
                acTransit = new AcTransitConsumer(apiKey);
                Log.Information("Created ACTransit GTFS-RT consumer.");
            }
            catch (Exception ex)
            {
                Log.Error("A(n) {ExceptionType} occured while accessing the ACTransit GTFS-RT endpoint: {ExceptionMessage}.", ex.GetType().Name, ex.Message);
            }
            IConsumerService service = new ConsumerService(acTransit, consumer);
            return service;
        }

        /// <summary>
        /// Creates and starts a <see cref="IConsumerBus"/> for 
        /// publishing messages.
        /// </summary>
        /// <returns>A new <see cref="IConsumerBus"/> for publishing
        /// messages down the system.</returns>
        private static async Task<IConsumerBus> CreateAndStartBus()
        {
            string rabbitUser = Environment.GetEnvironmentVariable("RABBIT_USER");
            string rabbitPass = Environment.GetEnvironmentVariable("RABBIT_PASS");
            IConsumerBus consumer = new ConsumerBus("rabbitmq.service", rabbitUser, rabbitPass);
            await consumer.Start();
            return consumer;
        }

        /// <summary>
        /// Initializes logging using Serilog and Seq.
        /// </summary>
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
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();
        }
    }
}
