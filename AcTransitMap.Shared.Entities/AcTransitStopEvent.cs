using GtfsConsumer.Entities.Interfaces;
using System;

namespace GtfsConsumer.Entities
{
    public class AcTransitStopEvent : IStopTimeEvent
    {
        public int Delay { get; set; }
        public DateTime Timestamp { get; set; }
    }
}