using GtfsConsumer.Entities;
using GtfsConsumer.Entities.Interfaces;
using GtfsConsumer.Interfaces;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TransitRealtime;

namespace GtfsConsumer
{
    public class AcTransitConsumer : ITransitConsumer
    {
        private readonly string _apiKey;

        public AcTransitConsumer(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        private async Task<FeedMessage> GatherMessage(string endpoint)
        {
            WebRequest webReq = HttpWebRequest.Create($"https://api.actransit.org/transit/gtfsrt/{endpoint}?token={_apiKey}");
            var response = await webReq.GetResponseAsync();
            FeedMessage message = Serializer.Deserialize<FeedMessage>(response.GetResponseStream());
            return message;
        }

        public async Task<IEnumerable<IVehiclePosition>> GetVehiclePositions()
        {
            return FormVehiclePositions(await GatherMessage("vehicles"));
        }

        private IEnumerable<IVehiclePosition> FormVehiclePositions(FeedMessage message)
        {
            List<IVehiclePosition> outVar = new List<IVehiclePosition>();

            foreach (var ent in message.Entities)
            {
                VehiclePosition pos = ent.Vehicle;
                AcTransitVehiclePosition ourPos = new AcTransitVehiclePosition()
                {
                    Bearing = (ushort)pos.Position.Bearing,
                    Latitude = pos.Position.Latitude,
                    Longitude = pos.Position.Longitude,
                    RouteId = pos.Trip?.RouteId,
                    Speed = pos.Position.Speed,
                    Timestamp = DateTime.UnixEpoch.AddSeconds(pos.Timestamp),
                    StopId = pos.StopId,
                    TripId = pos.Trip?.TripId,
                    VehicleId = pos.Vehicle.Id
                };
                outVar.Add(ourPos);
            }

            return outVar;
        }

        public IEnumerable<IAlert> GetAlerts()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ITripUpdate>> GetTripUpdates()
        {
            return FormUpdates(await GatherMessage("tripupdates"));
        }

        private IEnumerable<ITripUpdate> FormUpdates(FeedMessage message)
        {
            List<ITripUpdate> outVar = new List<ITripUpdate>();
            foreach (var msg in message.Entities)
            {
                var trip = msg.TripUpdate;
                ITripUpdate upd = CreateTripUpdate(trip);
                outVar.Add(upd);
            }
            return outVar;
        }

        private ITripUpdate CreateTripUpdate(TripUpdate trip)
        {
            return new AcTransitTripUpdate()
            {
                Delay = trip.Delay,
                Timestamp = DateTime.UnixEpoch.AddSeconds(trip.Timestamp),
                Trip = new AcTransitTripDescriptor()
                {
                    TripId = trip.Trip.TripId,
                    RouteId = trip.Trip.RouteId,
                    SheduleType = trip.Trip.schedule_relationship.ToString(),
                    DirectionId = trip.Trip.DirectionId
                },
                Vehicle = new AcTransitVehicleDescriptor()
                {
                    Id = trip.Vehicle.Id,
                    Label = trip.Vehicle.Label,
                    LicensePlate = trip.Vehicle.LicensePlate
                },
                StopTimeUpdates = GetStopTimeUpdates(trip)
            };
        }

        private IEnumerable<IStopTimeUpdate> GetStopTimeUpdates(TripUpdate trip)
        {
            List<IStopTimeUpdate> stopTimeUpd = new List<IStopTimeUpdate>();
            foreach (var stopTimes in trip.StopTimeUpdates)
            {
                IStopTimeUpdate stopTime = new AcTransitStopTime()
                {
                    StopId = stopTimes.StopId,
                    Arrival = new AcTransitStopEvent()
                    {
                        Delay = stopTimes.Arrival.Delay,
                        Timestamp = DateTime.UnixEpoch.AddSeconds(stopTimes.Arrival.Time)
                    },
                    Departure = new AcTransitStopEvent()
                    {
                        Delay = stopTimes.Departure.Delay,
                        Timestamp = DateTime.UnixEpoch.AddSeconds(stopTimes.Departure.Time)
                    }
                };
                stopTimeUpd.Add(stopTime);
            }
            return stopTimeUpd;
        }


    }
}
