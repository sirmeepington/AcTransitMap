using AcTransitMap.Services;
using GtfsConsumer.Entities.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AcTransitMap.Consumers
{
    public class VehiclePositionConsumer : IConsumer<IUpdatedVehiclePosition>
    {
        private readonly ILogger<VehiclePositionConsumer> _logger;
        private readonly IPositionService _positionService;

        public VehiclePositionConsumer(ILogger<VehiclePositionConsumer> logger, IPositionService positionService)
        {
            _logger = logger;
            _positionService = positionService;
        }

        public async Task Consume(ConsumeContext<IUpdatedVehiclePosition> context)
        {
            await _positionService.UpdateVehiclePosition(context.Message);
        }
    }
}
