﻿using GtfsConsumer.Entities.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtfsConsumer.Services
{
    /// <summary>
    /// Service to manaage the consuming of GTFS-RT data.
    /// </summary>
    public class ConsumerService
    {
        private readonly IConsumerBus _bus;
        private readonly ITransitConsumer _consumer;

        public ConsumerService(ITransitConsumer consumer, IConsumerBus bus)
        {
            _consumer = consumer;
            _bus = bus;
        }

        /// <summary>
        /// Timer callback for reading the vehicle positions from the
        /// GTFS feed and publishes it to the bus.
        /// </summary>
        /// <param name="state"></param>
        public async Task Publish()
        {
            Log.Information("Gathering GTFS-RT vehicle information");
            IEnumerable<IVehiclePosition> vehicles;
            try
            {
                vehicles = await _consumer.GetVehiclePositions();
                // Count casts to ICollection<T> for performance (will work as intended).
                Log.Information("Retrieved {VehicleAmount} vehicles from the GTFS-RT feed.", vehicles.Count());
            }
            catch (Exception ex)
            {
                Log.Error("A(n) {ExceptionType} occured while retrieving ACTransit vehicle positions: {ExceptionMessage}.", ex.GetType().Name, ex.Message);
                return;
            }
            Log.Information("Publishing gathered GTFS-RT vehicle information");
            foreach (IVehiclePosition pos in vehicles)
            {
                await _bus.Publish(pos);
            }

        }
    }
}
