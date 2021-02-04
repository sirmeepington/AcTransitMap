using Microsoft.AspNetCore.SignalR;

namespace AcTransitMap.Hubs
{
    /// <summary>
    /// SignalR hub for sending <see cref="AcTransitMap.Database.UpdatedVehiclePosition"/>
    /// updates to web clients in realtime.
    /// 
    /// Uses the generic client of <see cref="IPositionClient"/> for a 
    /// strongly-typed send method.
    /// </summary>
    public class MapPositionHub : Hub<IPositionClient>
    {
    }
}
