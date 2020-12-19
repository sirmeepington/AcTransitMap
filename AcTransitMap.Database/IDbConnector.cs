using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcTransitMap.Database
{
    public interface IDbConnector<TObj,TId> where TObj : class
    {
        Task<bool> Delete(TId id);
        Task<TObj> Get(TId id);
        Task<bool> Update(TId id, TObj obj);
        Task<IEnumerable<TObj>> GetAll();
    }
}