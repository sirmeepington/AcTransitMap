using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AcTransitMap.Services
{
    /// <summary>
    /// A hosted service for gathering initial values upon application
    /// start.
    /// 
    /// This is used as the <see cref="IPositionService"/> doesn't have
    /// a reasonable <see cref="StartAsync(CancellationToken)"/> method
    /// that can be used.
    /// </summary>
    public class PositionUpdaterService : IHostedService
    {
        private readonly IPositionService _positionService;

        public PositionUpdaterService(IPositionService positionService)
        {
            _positionService = positionService;
        }

        /// <summary>
        /// Runs <see cref="IPositionService.GatherInitialValuesAsync"/>
        /// upon start to gather initial database values before clients
        /// access the map.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
