using AcTransitMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcTransitMap.Hubs
{
    public interface IPositionClient
    {
        public Task UpdateLocation(VehiclePosition newLocation);

    }
}
