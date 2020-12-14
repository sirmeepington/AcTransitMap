using System;

namespace GtfsConsumer.Entities.Interfaces
{
    public interface IVehiclePosition
    {

        public string StopId { get; set; }

        public DateTime Timestamp { get; set; }

        public ushort Bearing { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public float Speed { get; set; }

        public string VehicleId { get; set; }

        public string RouteId { get; set; }

        public string TripId { get; set; }

    }
}