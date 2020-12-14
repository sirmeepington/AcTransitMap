using System.Threading.Tasks;

namespace GtfsConsumer
{
    public interface IConsumerBus
    {
        Task Publish<T>(T obj);
        Task Start();
        Task Stop();
    }
}