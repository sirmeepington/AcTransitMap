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
    public class VehiclePositionConsumer : IConsumer<IVehiclePosition>
    {
        private readonly IUpdaterService _updater;

        public VehiclePositionConsumer(IUpdaterService updater)
        {
            _updater = updater;
        }

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
