using AcTransitMap.Database;
using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MessageProcessor
{
    /// <inheritdoc cref="IUpdaterService"/>
    public class UpdaterService : IUpdaterService
    {

        private readonly IDbConnector<UpdatedVehiclePosition, string> _database;
        private readonly IBusControl _bus;

        public UpdaterService(IDbConnector<UpdatedVehiclePosition, string> database, IBusControl bus)
        {
            _database = database;
            _bus = bus;
        }

        /// <inheritdoc/>
        public async Task Update(IVehiclePosition message)
        {
            IUpdatedVehiclePosition newVehiclePos = new UpdatedVehiclePosition()
            {
                VehicleId = message.VehicleId,
                Timestamp = DateTime.UtcNow,
                Latitude = message.Latitude,
                Longitude = message.Longitude
            };

            // Update
            try
            {
                // TODO: Check here if the data _needs_ updating before updating.
                await _database.Update(message.VehicleId, (UpdatedVehiclePosition)newVehiclePos);

                await _bus.Publish(newVehiclePos);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update vehicle position for {VehicleId}: {ExceptionMessage}", message.VehicleId, ex.Message);
            }
        }


    }
}
