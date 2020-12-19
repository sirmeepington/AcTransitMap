using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcTransitMap.Database
{
    public class MongoDbConnector : IDbConnector<UpdatedVehiclePosition,string>
    {
        private readonly IMongoCollection<UpdatedVehiclePosition> _collection;

        public MongoDbConnector(string connectionInfo, string dbName, string collectionName)
        {
            if (string.IsNullOrEmpty(connectionInfo))
            {
                throw new System.ArgumentException($"'{nameof(connectionInfo)}' cannot be null or empty", nameof(connectionInfo));
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new System.ArgumentException($"'{nameof(dbName)}' cannot be null or empty", nameof(dbName));
            }

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new System.ArgumentException($"'{nameof(collectionName)}' cannot be null or empty", nameof(collectionName));
            }

            IMongoClient client = new MongoClient(connectionInfo);
            IMongoDatabase db = client.GetDatabase(dbName);
            _collection = db.GetCollection<UpdatedVehiclePosition>(collectionName);
        }

        public async Task<bool> Delete(string id)
        {
            var res = await _collection.DeleteOneAsync(id);
            return res.IsAcknowledged;
        }

        public async Task<UpdatedVehiclePosition> Get(string id)
        {
            var op = await _collection.FindAsync(x => x.VehicleId == id);
            return await op.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UpdatedVehiclePosition>> GetAll()
        {
            var res = await _collection.FindAsync(_ => true);
            return await res.ToListAsync();
        }

        public async Task<bool> Update(string id, UpdatedVehiclePosition obj)
        {
            var op = await _collection.ReplaceOneAsync(x => x.VehicleId == id, obj, new ReplaceOptions() { IsUpsert = true });
            return op.IsAcknowledged;
        }
    }
}
