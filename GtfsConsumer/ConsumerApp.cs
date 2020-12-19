using GtfsConsumer.Entities;
using GtfsConsumer.Entities.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
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

            string apiKey = Environment.GetEnvironmentVariable("ACTRANSIT_KEY");
            string user = Environment.GetEnvironmentVariable("RABBIT_USER");
            string pass = Environment.GetEnvironmentVariable("RABBIT_PASS");

            IConsumerBus consumer = new ConsumerBus("rabbitmq.service",user,pass);
            await consumer.Start();

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

            List<IVehiclePosition> vehicles;
            try
            {
                vehicles = (List<IVehiclePosition>) await acTransit.GetVehiclePositions();
                Log.Information("Retrieved {VehicleAmount} vehicles from the GTFS-RT feed.", vehicles.Count);
            }
            catch (Exception ex)
            {
                Log.Error("A(n) {ExceptionType} occured while retrieving ACTransit vehicle positions: {ExceptionMessage}.", ex.GetType().Name, ex.Message);
                return;
            }

            Log.Information("Publishing {VehicleAmount} vehicles to the message queue.",vehicles.Count);
            foreach (IVehiclePosition pos in vehicles)
            {
                await consumer.Publish(pos);
            }

            try
            {
                Console.WriteLine("Done.");
            }
            finally
            {
                await consumer.Stop();
            }
        }
    }
}
