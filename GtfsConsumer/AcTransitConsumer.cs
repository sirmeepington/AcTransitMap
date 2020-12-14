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
            _apiKey = apiKey;
        }

        public async Task<IEnumerable<IVehiclePosition>> GetVehiclePositions()
        {
            WebRequest webReq = HttpWebRequest.Create($"https://api.actransit.org/transit/gtfsrt/vehicles?token={_apiKey}");
            var response = await webReq.GetResponseAsync();
            FeedMessage message = Serializer.Deserialize<FeedMessage>(response.GetResponseStream());
            return FormVehiclePositions(message);
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

        public IEnumerable<ITripUpdate> GetTripUpdates()
        {
            WebRequest req = HttpWebRequest.Create($"https://api.actransit.org/transit/gtfsrt/tripupdates?token={_apiKey}");
            FeedMessage message = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
            return FormUpdates(message);
        }

        private IEnumerable<ITripUpdate> FormUpdates(FeedMessage message)
        {
            List<ITripUpdate> outVar = new List<ITripUpdate>();
            foreach (var msg in message.Entities)
            {
                var trip = msg.TripUpdate;

                ITripUpdate upd = new AcTransitTripUpdate()
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
                outVar.Add(upd);
            }
            return outVar;
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
