using AcTransitMap.Database;
using AcTransitMap.Hubs;
using AcTransitMap.Models;
using GtfsConsumer.Entities.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AcTransitMap.Services
{
    /// <inheritdoc cref="IPositionService"/>
    public class PositionService : IPositionService
    {
        private readonly ConcurrentDictionary<string, VehiclePosition> _positions;
        private readonly ILogger<PositionService> _logger;
        private readonly IDbConnector<UpdatedVehiclePosition, string> _database;
        private readonly IHubContext<MapPositionHub,IPositionClient> _posHub;

        private bool gathered = false;

        public PositionService(ILogger<PositionService> logger, IDbConnector<UpdatedVehiclePosition, string> database, IHubContext<MapPositionHub,IPositionClient> posHub)
        {
            _positions = new ConcurrentDictionary<string, VehiclePosition>();
            _logger = logger;
            _database = database;
            _posHub = posHub;
        }

        /// <inheritdoc/>
        public async Task GatherInitialValuesAsync()
        {
            if (gathered)
                return;
            // TODO: Make this gather the initial values from the database to populate before messages come through.
            // This should be faster than the time it takes for messages to propogate and process; otherwise there
            // may be some conflicts on data.
            IEnumerable<UpdatedVehiclePosition> positions = await _database.GetAll();

            foreach(UpdatedVehiclePosition vehiclePos in positions)
            {
                VehiclePosition newPos = new VehiclePosition()
                {
                    VehicleId = vehiclePos.VehicleId,
                    LastUpdated = DateTime.UtcNow.ToString("G"),
                    Latitude = vehiclePos.Latitude,
                    Longitude = vehiclePos.Longitude
                };
                _positions.TryAdd(vehiclePos.VehicleId, newPos);
            }
            gathered = true;
        }

        /// <inheritdoc/>
        public IEnumerable<VehiclePosition> GetPositions()
        {
            return _positions.Values.ToList();
        }

        /// <inheritdoc/>
        public async Task UpdateVehiclePosition(IUpdatedVehiclePosition pos)
        {
            if (!_positions.TryGetValue(pos.VehicleId, out VehiclePosition vehiclePos))
            {
                vehiclePos = new VehiclePosition
                {
                    VehicleId = pos.VehicleId
                };
                _logger.LogDebug("Created object for new vehicle {VehicleId}.", pos.VehicleId);
            }

            vehiclePos.Latitude = pos.Latitude;
            vehiclePos.Longitude = pos.Longitude;
            vehiclePos.LastUpdated = DateTime.UtcNow.ToString("G");

            _positions[pos.VehicleId] = vehiclePos;
//            _logger.LogDebug("Updated postition for {VehicleId}: {Longitude}, {Latitude} at {DateTime}", pos.VehicleId, pos.Longitude, pos.Latitude,DateTime.UtcNow.ToString());

            await _posHub.Clients.All.UpdateLocation(vehiclePos);
        }

    }
}
