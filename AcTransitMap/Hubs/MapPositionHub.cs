using AcTransitMap.Models;
using AcTransitMap.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcTransitMap.Hubs
{
    public class MapPositionHub : Hub
    {

        public async Task UpdateLocations(IEnumerable<VehiclePosition> newLocations)
        {
            await Clients.All.SendAsync("UpdateLocations", newLocations);
        }
    }
}
