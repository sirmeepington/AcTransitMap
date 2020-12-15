using AcTransitMap.Models;
using GtfsConsumer.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AcTransitMap.Services
{
    public class PositionService : IPositionService
    {
        private readonly ConcurrentDictionary<string, VehiclePosition> _positions;
        private readonly ILogger<PositionService> _logger;

        public PositionService(ILogger<PositionService> logger)
        {
            _logger = logger;
            _positions = new ConcurrentDictionary<string, VehiclePosition>();
        }

        public IEnumerable<VehiclePosition> GetPositions()
        {
            return _positions.Values.ToList();
        }

        public void UpdateVehiclePosition(IVehiclePosition pos)
        {
            if (!_positions.TryGetValue(pos.VehicleId, out VehiclePosition vehiclePos))
            {
                vehiclePos = new VehiclePosition();
                vehiclePos.VehicleId = pos.VehicleId;
            }

            vehiclePos.Latitude = pos.Latitude;
            vehiclePos.Longitude = pos.Longitude;
            vehiclePos.LastUpdated = DateTime.UtcNow.ToString("G");

            _positions[pos.VehicleId] = vehiclePos;
        }

    }
}
