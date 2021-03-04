using GtfsConsumer.Entities.Interfaces;
using System.Threading.Tasks;

namespace MessageProcessor
{
    /// <summary>
    /// A service for updating recieved <see cref="IVehiclePosition"/> messages.
    /// <br/>
    /// This service assumes that the messages pushed have already been validated
    /// by the receiving consumer.
    /// </summary>
    public interface IUpdaterService
    {
        /// <summary>
        /// Processes an <see cref="IVehiclePosition"/> message and 
        /// sends it down the bus.
        /// </summary>
        /// <param name="message">The <see cref="IVehiclePosition"/> to validate 
        /// and update.</param>
        /// <returns>An awaitable task for processing and updating the message.</returns>
        Task Update(IVehiclePosition message);
    }
}