using CreolytixECommerce.Domain.Entities;
using CreolytixECommerce.Domain.Interfaces;
using CreolytixECommerce.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CreolytixECommerce.Infrastructure.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly IMongoCollection<Store> _storeCollection;

        public StoreRepository(MongoDbContext context)
        {
            _storeCollection = context.GetCollection<Store>("stores");
        }

        public async Task<Store> GetByIdAsync(string storeId)
        {
            return await _storeCollection.Find(s => s.Id == storeId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Store>> GetNearbyStoresAsync(double latitude, double longitude, double radius)
        {
            double radiusInRadians = radius / 6378.1;
            var filter = Builders<Store>.Filter.GeoWithinCenterSphere(
                x => x.Location.Coordinates,
                longitude,
                latitude,
                radiusInRadians
            );
            return await _storeCollection.Find(filter).ToListAsync();
        }
    }
}
