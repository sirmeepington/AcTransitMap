﻿using AcTransitMap.Models;
using GtfsConsumer.Entities.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AcTransitMap.Services
{
    public class PositionService : IPositionService
    {
        private readonly ConcurrentDictionary<string, VehiclePosition> _positions;

        public PositionService()
        {
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
                vehiclePos = new VehiclePosition
                {
                    VehicleId = pos.VehicleId
                };
            }

            vehiclePos.Latitude = pos.Latitude;
            vehiclePos.Longitude = pos.Longitude;
            vehiclePos.LastUpdated = DateTime.UtcNow.ToString("G");

            _positions[pos.VehicleId] = vehiclePos;
        }

    }
}
