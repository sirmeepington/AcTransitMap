using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AcTransitMap.Services
{
    public class PositionUpdaterService : IHostedService
    {
        private readonly IPositionService _positionService;

        public PositionUpdaterService(IPositionService positionService)
        {
            _positionService = positionService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _positionService.GatherInitialValuesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
