using AcTransitMap.Models;
using GtfsConsumer.Entities.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcTransitMap.Services
{
    /// <summary>
    /// A service for gathering and updating vehicle positions.
    /// </summary>
    public interface IPositionService
    {
        /// <summary>
        /// Updates a vehicle position from the 
        /// <see cref="IUpdatedVehiclePosition"/> message
        /// received form the message bus.
        /// </summary>
        /// <param name="pos">The <see cref="IUpdatedVehiclePosition"/>
        /// object to update from.</param>
        /// <returns>An awaitable task which updates the position.</returns>
        Task UpdateVehiclePosition(IUpdatedVehiclePosition pos);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="VehiclePosition"/>s.
        /// 
        /// This should use an external database for contingency.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="VehiclePosition"/>
        /// objects for all the vehicles currently stored.</returns>
        IEnumerable<VehiclePosition> GetPositions();

        /// <summary>
        /// Gathers initial values upon service start from the database.
        /// This is to avoid having to wait for all the vehicles to
        /// parse through the message bus to from missing some vehicles
        /// that haven't parsed through.
        /// </summary>
        /// <returns>An awaitable task that gathers vehicle information.</returns>
        Task GatherInitialValuesAsync();
    }
}