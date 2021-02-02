using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GtfsConsumer
{
    /// <summary>
    /// Consumer bus implementation.
    /// </summary>
    public class ConsumerBus : IConsumerBus
    {
        private readonly IBusControl _bus;

        public ConsumerBus(string host, string rabbitUser, string rabbitPass)
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(bus =>
            {
                bus.Host(host, "/", rabbit =>
                {
                    rabbit.Username(rabbitUser);
                    rabbit.Password(rabbitPass);
                });
            });
        }

        /// <summary>
        /// Starts the bus.
        /// </summary>
        public async Task Start()
        {
            await _bus.StartAsync();
        }

        /// <summary>
        /// Stops the bus.
        /// </summary>
        public async Task Stop()
        {
            await _bus.StopAsync();
        }

        /// <summary>
        /// Publishes a message to the bus.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="obj">The message object.</param>
        public async Task Publish<T>(T obj)
        {
            await _bus.Publish(obj);
        }
    }
}
