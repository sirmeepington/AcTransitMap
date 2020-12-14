using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GtfsConsumer
{
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
                bus.Publish<IVehiclePositionUpdate>(x =>
                {
                    x.ExchangeType = "fanout";
                });
            });
        }

        public async Task Start()
        {
            await _bus.StartAsync();
        }

        public async Task Stop()
        {
            await _bus.StopAsync();
        }

        public async Task Publish<T>(T obj)
        {
            await _bus.Publish(obj);
        }
    }
}
