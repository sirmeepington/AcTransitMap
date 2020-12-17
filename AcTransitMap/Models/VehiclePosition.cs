using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcTransitMap.Models
{
    public class VehiclePosition
    {

        public string VehicleId { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string LastUpdated { get; set; }
    }
}
