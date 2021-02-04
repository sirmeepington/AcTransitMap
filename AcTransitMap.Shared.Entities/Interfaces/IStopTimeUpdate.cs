namespace GtfsConsumer.Entities.Interfaces
{ 
    public interface IStopTimeUpdate
    {

        string StopId { get; set; }

        IStopTimeEvent Arrival { get; set; }

        IStopTimeEvent Departure { get; set; }

        uint StopSequence { get; set; }

    }
}