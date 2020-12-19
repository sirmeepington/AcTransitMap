using System;
using System.Collections.Generic;
using System.Text;

namespace GtfsConsumer.Entities.Interfaces
{
    public interface IUpdatedVehiclePosition
    {

        public DateTime Timestamp { get; set; }

        public ushort Bearing { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public float Speed { get; set; }

        public string VehicleId { get; set; }

    }
}
