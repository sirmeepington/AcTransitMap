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
            ConsumerService service = CreateService(consumer);

            try
            {
                // Call every 30s
                Log.Information("Beginning GTFS-RT gather timer.");
                Timer timer = new Timer(new TimerCallback(service.Publish), null, 0, GetDelay());
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
        /// Gets a delay in seconds from the "FETCH_DELAY" environment variable.
        /// Multiplies it by 1000 to make it into milliseconds and returns in.
        /// </summary>
        /// <returns>The second delay in milliseconds if valid; or 10 seconds in
        /// milliseconds if invalid.</returns>
        private static int GetDelay()
        {
            string delayEnvVar = Environment.GetEnvironmentVariable("FETCH_DELAY");
            if (float.TryParse(delayEnvVar, out float delayFloat))
            {
                return (int)(delayFloat * 1000); // In seconds.
            }
            Log.Warning("Invalid FETCH_DELAY ({InvalidDelay}) given. Expected numeric value.");
            return 10000;
        }

        /// <summary>
        /// Creates a <see cref="ConsumerService"/> from the given
        /// <see cref="IConsumerBus"/>.
        /// </summary>
        /// <param name="consumer">The consumer bus to create from.</param>
        private static ConsumerService CreateService(IConsumerBus consumer)
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
            ConsumerService service = new ConsumerService(acTransit, consumer);
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
            string rabbitUrl = Environment.GetEnvironmentVariable("RABBIT_URL");
            IConsumerBus consumer = new ConsumerBus(rabbitUrl, rabbitUser, rabbitPass);
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
