using GtfsConsumer.Entities.Interfaces;
using System;

namespace AcTransitMap.Database
{
    public class UpdatedVehiclePosition : IUpdatedVehiclePosition
    {
        public DateTime Timestamp { get; set; }
        public ushort Bearing { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Speed { get; set; }
        public string VehicleId { get; set; }
    }
}
