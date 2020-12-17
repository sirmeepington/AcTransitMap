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
        private readonly IPositionService _positions;

        public MapController(IPositionService positions)
        {
            _positions = positions;
        }

        public IActionResult Index()
        {
            IEnumerable<VehiclePosition> model = _positions.GetPositions();
            return View(model);
        } 
    }
}
