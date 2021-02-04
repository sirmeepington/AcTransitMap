using GtfsConsumer.Entities.Interfaces;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AcTransitMap.Database
{
    public class UpdatedVehiclePosition : IUpdatedVehiclePosition
    {
        [BsonIgnoreIfNull]
        public BsonObjectId Id { get; set; }
        public DateTime Timestamp { get; set; }
        public ushort Bearing { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Speed { get; set; }
        public string VehicleId { get; set; }
    }
}
