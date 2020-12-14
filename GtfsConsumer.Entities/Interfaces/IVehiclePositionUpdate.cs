using System;
using System.Collections.Generic;
using System.Text;

namespace GtfsConsumer.Entities.Interfaces
{
    public interface IVehiclePositionUpdate
    {

        public List<IVehiclePosition> Positions { get; set; }

    }
}
