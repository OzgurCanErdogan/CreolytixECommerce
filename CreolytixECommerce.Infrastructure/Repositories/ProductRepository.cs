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
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _productCollection;

        public ProductRepository(MongoDbContext context)
        {
            _productCollection = context.GetCollection<Product>("products");
        }

        // Retrieves a product by ID
        public async Task<Product> GetByIdAsync(string productId)
        {
            return await _productCollection.Find(p => p.Id == productId).FirstOrDefaultAsync();
        }

        // Retrieves products by category
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Category, category);
            return await _productCollection.Find(filter).ToListAsync();
        }

    }
}

