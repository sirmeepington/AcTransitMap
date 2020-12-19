using AcTransitMap.Models;
using AcTransitMap.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AcTransitMap.Controllers
{
    public class MapController : Controller
    {
        private readonly ILogger<MapController> _logger;
        private readonly IPositionService _positions;

        public MapController(ILogger<MapController> logger, IPositionService positions)
        {
            _logger = logger;
            _positions = positions;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Serving GET {Url}.", "/Map/Index");
            IEnumerable<VehiclePosition> model = _positions.GetPositions();
            return View(model);
        } 
    }
}
