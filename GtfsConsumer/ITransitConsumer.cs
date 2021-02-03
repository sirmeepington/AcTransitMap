using GtfsConsumer.Entities;
using GtfsConsumer.Entities.Interfaces;
using GtfsConsumer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GtfsConsumer
{
    /// <summary>
    /// A consumer interface which defines methods for gathering data from a
    /// GTFS-RT feed.
    /// </summary>
    public interface ITransitConsumer
    {
        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="IAlert"/>s from
        /// the GTFS feed.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="IAlert"/>s.</returns>
        IEnumerable<IAlert> GetAlerts();

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="ITripUpdate"/>s
        /// from the GTFS feed.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ITripUpdate"/>s.</returns>
        Task<IEnumerable<ITripUpdate>> GetTripUpdates();

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="IVehiclePosition"/>s
        /// from the GTFS feed.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IVehiclePosition"/>s.</returns>
        Task<IEnumerable<IVehiclePosition>> GetVehiclePositions();
    }
}