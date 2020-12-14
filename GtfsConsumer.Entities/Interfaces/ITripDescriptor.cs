using System;

namespace GtfsConsumer.Entities.Interfaces
{
    public interface ITripDescriptor
    {

        public uint DirectionId { get; set; }

        public string RouteId { get; set; }

        public DateTime Start { get; set; }

        public string TripId { get; set; }

        public string SheduleType { get; set; }

    }
}