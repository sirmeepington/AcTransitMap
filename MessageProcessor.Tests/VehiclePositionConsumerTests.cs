using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Threading.Tasks;

namespace MessageProcessor.Tests
{
    [TestFixture]
    public class VehiclePositionConsumerTests
    {

        [TestCase("Test",-10,-10,false)]
        [TestCase(null,-10,-10,true)]
        [TestCase("Test",9999,99999,true)]
        public async Task ShouldParse(string vehicleId, float longitude, float latitude, bool invalidMessage)
        {

            var harness = new InMemoryTestHarness();
            var updaterService = new Mock<IUpdaterService>();
            var message = new TestVehiclePosition()
            {
                VehicleId = vehicleId,
                Bearing = 0,
                Latitude = longitude,
                Longitude = latitude,
                Speed = 0,
                Timestamp = DateTime.UtcNow
            };

            var consumer = harness.Consumer(() => new VehiclePositionConsumer(updaterService.Object));

            await harness.Start();

            Constraint result = Is.False;
            if (invalidMessage)
                result = Is.True;

            try
            {
                await harness.InputQueueSendEndpoint.Send(message);
                Assert.That(await consumer.Consumed.Any<IVehiclePosition>());
                Assert.That(await harness.Published.Any<Fault<IVehiclePosition>>(), result);
            } 
            finally
            {
                await harness.Stop();
            }
        }

        /// <summary>
        /// An example test for unit testing <see cref="VehiclePositionConsumer"/>
        /// with a <see cref="IVehiclePosition"/> due to issues
        /// with Newtonsoft JSON and Moq.
        /// </summary>
        public class TestVehiclePosition : IVehiclePosition
        {
            public DateTime Timestamp { get; set; }
            public ushort Bearing { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public float Speed { get; set; }
            public string VehicleId { get; set; }
        }
    }
}