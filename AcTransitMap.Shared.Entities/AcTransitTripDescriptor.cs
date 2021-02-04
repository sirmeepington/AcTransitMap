using GtfsConsumer.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GtfsConsumer.Entities
{
    public class AcTransitTripDescriptor : ITripDescriptor
    {
        public uint DirectionId { get; set; }
        public string RouteId { get; set; }
        public DateTime Start { get; set; }
        public string TripId { get; set; }
        public string SheduleType { get; set; }
    }
}
