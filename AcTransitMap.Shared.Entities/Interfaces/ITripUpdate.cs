using System;
using System.Collections.Generic;

namespace GtfsConsumer.Entities.Interfaces
{
    public interface ITripUpdate
    {
        int Delay { get; set; }
        DateTime Timestamp { get; set; }
        ITripDescriptor Trip { get; set; }
        IVehicleDescriptor Vehicle { get; set; }
        IEnumerable<IStopTimeUpdate> StopTimeUpdates { get; set; }
    }
}