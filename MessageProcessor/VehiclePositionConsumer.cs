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
        private readonly IDbConnector<UpdatedVehiclePosition, string> _database;
        private readonly IBusControl _bus;

        public VehiclePositionConsumer(IDbConnector<UpdatedVehiclePosition, string> database, IBusControl bus)
        {
            _database = database;
            _bus = bus;
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

            UpdatedVehiclePosition existing = await _database.Get(message.VehicleId);

            IUpdatedVehiclePosition newVehiclePos = new UpdatedVehiclePosition()
            {
                VehicleId = message.VehicleId,
                Timestamp = DateTime.UtcNow,
                Latitude = message.Latitude,
                Longitude = message.Longitude
            };

            // Update
            await _database.Update(message.VehicleId, (UpdatedVehiclePosition) newVehiclePos);

            await _bus.Publish(newVehiclePos);

        }
    }
}
