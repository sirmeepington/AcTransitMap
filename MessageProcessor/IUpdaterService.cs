using GtfsConsumer.Entities.Interfaces;
using System.Threading.Tasks;

namespace MessageProcessor
{
    public interface IUpdaterService
    {
        Task Update(IVehiclePosition message);
    }
}