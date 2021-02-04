using System;

namespace GtfsConsumer.Entities.Interfaces
{
    public interface IStopTimeEvent
    {

        int Delay { get; set; }

        DateTime Timestamp { get; set; }

    }
}