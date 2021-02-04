using GtfsConsumer.Entities.Interfaces;

namespace GtfsConsumer.Entities
{
    public class AcTransitStopTime : IStopTimeUpdate
    {
        public string StopId { get; set; }
        public IStopTimeEvent Arrival { get; set; }
        public IStopTimeEvent Departure { get; set; }
        public uint StopSequence { get; set; }
    }
}