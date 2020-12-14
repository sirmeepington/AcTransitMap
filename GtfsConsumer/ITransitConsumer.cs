using GtfsConsumer.Entities;
using GtfsConsumer.Entities.Interfaces;
using GtfsConsumer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GtfsConsumer
{
    public interface ITransitConsumer
    {
        IEnumerable<IAlert> GetAlerts();
        IEnumerable<ITripUpdate> GetTripUpdates();
        Task<IEnumerable<IVehiclePosition>> GetVehiclePositions();
    }
}