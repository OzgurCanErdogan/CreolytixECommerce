using CreolytixECommerce.Domain.Entities;
using CreolytixECommerce.Domain.Interfaces;
using CreolytixECommerce.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IMongoCollection<Inventory> _inventoryCollection;

        public InventoryRepository(MongoDbContext database)
        {
            // Assume "Inventories" is the collection name
            _inventoryCollection = database.GetCollection<Inventory>("inventories");
        }

        // Retrieve inventory by store and product ID
        public async Task<Inventory> GetInventoryAsync(string storeId, string productId)
        {
            // Filter to match both StoreId and ProductId
            var filter = Builders<Inventory>.Filter.And(
                Builders<Inventory>.Filter.Eq(i => i.StoreId, storeId),
                Builders<Inventory>.Filter.Eq(i => i.ProductId, productId)
            );

            return await _inventoryCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Update an existing inventory document
        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            var filter = Builders<Inventory>.Filter.And(
                Builders<Inventory>.Filter.Eq(i => i.StoreId, inventory.StoreId),
                Builders<Inventory>.Filter.Eq(i => i.ProductId, inventory.ProductId)
            );

            // Replace the existing document with the updated inventory
            await _inventoryCollection.ReplaceOneAsync(filter, inventory);
        }

        public async Task<IEnumerable<Inventory>> GetInventoryByStoreIdAsync(string storeId)
        {
            var filter = Builders<Inventory>.Filter.Eq(i => i.StoreId, storeId);
            return await _inventoryCollection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetInventoryByProductIdAsync(string productId)
        {
            var filter = Builders<Inventory>.Filter.Eq(i => i.ProductId, productId);
            return await _inventoryCollection.Find(filter).ToListAsync();
        }

        public async Task CreateInventoryAsync(Inventory inventory)
        {
            await _inventoryCollection.InsertOneAsync(inventory);
        }
    }
}
