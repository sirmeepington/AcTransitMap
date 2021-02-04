using AcTransitMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcTransitMap.Hubs
{
    /// <summary>
    /// Position update client interface for defining a 
    /// strongly-typed <see cref="UpdateLocation(VehiclePosition)"/>
    /// method when accessing the <see cref="MapPositionHub"/>
    /// SignalR hub.
    /// </summary>
    public interface IPositionClient
    {
        /// <summary>
        /// Publishes a <see cref="VehiclePosition"/> update
        /// to clients through the <see cref="MapPositionHub"/>.
        /// 
        /// Only publishes an update for one vehicle update.
        /// </summary>
        /// <param name="newLocation">The new <see cref="VehiclePosition"/>
        /// position to send to clients.</param>
        /// <returns>An awaitable task.</returns>
        public Task UpdateLocation(VehiclePosition newLocation);

    }
}
