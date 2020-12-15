using AcTransitMap.Models;
using GtfsConsumer.Entities.Interfaces;
using System.Collections.Generic;

namespace AcTransitMap.Services
{
    public interface IPositionService
    {
        void UpdateVehiclePosition(IVehiclePosition pos);
        IEnumerable<VehiclePosition> GetPositions();
    }
}