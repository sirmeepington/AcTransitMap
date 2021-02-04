using GtfsConsumer.Entities.Interfaces;

namespace GtfsConsumer.Entities
{
    public class AcTransitVehicleDescriptor : IVehicleDescriptor
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string LicensePlate { get; set; }
    }
}