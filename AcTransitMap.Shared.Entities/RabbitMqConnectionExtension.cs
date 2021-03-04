using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;
using System.Text;

namespace AcTransitMap.Shared.Entities
{
    public static class RabbitMqConnectionExtension
    {

        /// <summary>
        /// Runs <see cref="IRabbitMqBusFactoryConfigurator.Host(string,string,RabbitMqHostSettings)"/>
        /// with the specified <paramref name="connection"/> info.
        /// </summary>
        /// <param name="rabbit">The <see cref="IRabbitMqBusFactoryConfigurator"/> to configure</param>
        /// <param name="connection">The <see cref="RabbitConnection"/> information
        /// to connect with.</param>
        public static void Host(this IRabbitMqBusFactoryConfigurator rabbit, RabbitConnection connection)
        {
            if (!connection.IsValid())
                return;

            rabbit.Host(connection.Endpoint,connection.VirtualHost, settings => {
                settings.Username(connection.Username);
                settings.Password(connection.Password);
            });
        }

    }
}
