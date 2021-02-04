using GtfsConsumer.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GtfsConsumer.Entities
{
    public class AcTransitTripUpdate : ITripUpdate
    {

        public int Delay { get; set; }

        public DateTime Timestamp { get; set; }

        public ITripDescriptor Trip { get; set; }

        public IVehicleDescriptor Vehicle { get; set; }

        public IEnumerable<IStopTimeUpdate> StopTimeUpdates { get; set; }
    }
}
