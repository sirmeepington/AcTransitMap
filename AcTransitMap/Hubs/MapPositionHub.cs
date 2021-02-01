using Microsoft.AspNetCore.SignalR;

namespace AcTransitMap.Hubs
{
    public class MapPositionHub : Hub<IPositionClient>
    {
    }
}
