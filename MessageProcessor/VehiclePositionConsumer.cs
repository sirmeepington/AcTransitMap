using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using AcTransitMap.Database;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessor
{
    /// <summary>
    /// A consumer for recieving raw <see cref="IVehiclePosition"/> messages
    /// from the GTFS-RT source.
    /// <br/>
    /// Each message received is validated here before
    /// being pushed through to the <see cref="IUpdaterService"/>.
    /// </summary>
    public class VehiclePositionConsumer : IConsumer<IVehiclePosition>
    {
        private readonly IUpdaterService _updater;

        public VehiclePositionConsumer(IUpdaterService updater)
        {
            _updater = updater;
        }

        /// <summary>
        /// Consumes the <see cref="IVehiclePosition"/> message and
        /// if valid pushes it to the <see cref="IUpdaterService"/>.
        /// </summary>
        /// <param name="context">The <see cref="ConsumeContext"/> for the
        /// current received message.</param>
        /// <returns>An awaitable task for consuming a message.</returns>
        public async Task Consume(ConsumeContext<IVehiclePosition> context)
        {
            // Validate
            // Run Mongo operations
            IVehiclePosition message = context.Message;
            if (!message.IsValid(out string msg))
            {
                Log.Error("Invalid message for vehicle {VehicleId} passed. {Message}", message.VehicleId ?? "No Vehicle Id", msg);
                return;
            }

            await _updater.Update(context.Message);
        }
    }
}
