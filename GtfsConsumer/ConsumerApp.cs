using GtfsConsumer.Entities;
using GtfsConsumer.Entities.Interfaces;
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

            string apiKey = Environment.GetEnvironmentVariable("ACTRANSIT_KEY");
            string user = Environment.GetEnvironmentVariable("RABBIT_USER");
            string pass = Environment.GetEnvironmentVariable("RABBIT_PASS");

            IConsumerBus consumer = new ConsumerBus("rabbitmq.service",user,pass);
            await consumer.Start();

            ITransitConsumer acTransit = new AcTransitConsumer(apiKey);

            IEnumerable<IVehiclePosition> vehicles = await acTransit.GetVehiclePositions();

            Console.WriteLine("Publishing");

            foreach(IVehiclePosition pos in vehicles)
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
