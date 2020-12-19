﻿using GtfsConsumer.Entities.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GtfsConsumer.Services
{
    public class ConsumerService
    {
        private readonly IConsumerBus _bus;
        private readonly ITransitConsumer _consumer;

        public ConsumerService(ITransitConsumer consumer, IConsumerBus bus)
        {
            _consumer = consumer;
            _bus = bus;
        }

        public async void Publish(object state)
        {
            List<IVehiclePosition> vehicles;
            try
            {
                vehicles = (List<IVehiclePosition>)await _consumer.GetVehiclePositions();
                Log.Information("Retrieved {VehicleAmount} vehicles from the GTFS-RT feed.", vehicles.Count);
            }
            catch (Exception ex)
            {
                Log.Error("A(n) {ExceptionType} occured while retrieving ACTransit vehicle positions: {ExceptionMessage}.", ex.GetType().Name, ex.Message);
                return;
            }
            foreach (IVehiclePosition pos in vehicles)
            {
                await _bus.Publish(pos);
            }

        }
    }
}
