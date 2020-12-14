namespace GtfsConsumer.Entities.Interfaces
{
    public interface IVehicleDescriptor
    {

        public string Id { get; set; }

        public string Label { get; set; }

        public string LicensePlate { get; set; }

    }
}