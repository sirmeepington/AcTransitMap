using System.Threading.Tasks;

namespace GtfsConsumer
{
    /// <summary>
    /// An interface declaring a consumer bus.
    /// </summary>
    public interface IConsumerBus
    {
        /// <summary>
        /// Publishes an object to the message bus.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to publish.</param>
        /// <returns>An awaitable task for publishing the object
        /// to the bus.</returns>
        Task Publish<T>(T obj);

        /// <summary>
        /// Starts the message bus.
        /// </summary>
        /// <returns>An awaitable task which starts the bus.</returns>
        Task Start();

        /// <summary>
        /// Stops the message bus.
        /// </summary>
        /// <returns>An awaitable task whichs stops the bus.</returns>
        Task Stop();
    }
}