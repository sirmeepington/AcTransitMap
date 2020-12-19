using GtfsConsumer.Entities;
using GtfsConsumer.Entities.Interfaces;
using GtfsConsumer.Services;
using Serilog;
using System;
using System.Collections.Generic;
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
            ConsumerService service = new ConsumerService(acTransit,consumer);

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

        private static async Task<IConsumerBus> CreateAndStartBus()
        {

            string rabbitUser = Environment.GetEnvironmentVariable("RABBIT_USER");
            string rabbitPass = Environment.GetEnvironmentVariable("RABBIT_PASS");
            IConsumerBus consumer = new ConsumerBus("rabbitmq.service", rabbitUser, rabbitPass);
            await consumer.Start();
            return consumer;
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
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();
        }
    }
}
