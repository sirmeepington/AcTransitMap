namespace GtfsConsumer.Services
{
    /// <summary>
    /// A consumer service which defines a callback method for publishing to a bus.
    /// </summary>
    public interface IConsumerService
    {
        /// <summary>
        /// Gathers and publishes data from the feed and publishes it to a bus.
        /// </summary>
        /// <param name="state">An optional state parameter from the timer object.</param>
        void Publish(object state);
    }
}