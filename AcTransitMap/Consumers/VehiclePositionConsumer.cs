using AcTransitMap.Services;
using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AcTransitMap.Consumers
{
    public class VehiclePositionConsumer : IConsumer<IVehiclePosition>
    {
        private readonly IPositionService _positionService;

        public VehiclePositionConsumer(IPositionService positionService)
        {
            _positionService = positionService;
        }

        public Task Consume(ConsumeContext<IVehiclePosition> context)
        {
            _positionService.UpdateVehiclePosition(context.Message);
            return Task.CompletedTask;
        }
    }
}
