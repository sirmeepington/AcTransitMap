using AcTransitMap.Models;
using GtfsConsumer.Entities.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcTransitMap.Services
{
    public interface IPositionService
    {
        void UpdateVehiclePosition(IUpdatedVehiclePosition pos);
        IEnumerable<VehiclePosition> GetPositions();
        Task GatherInitialValuesAsync();
    }
}