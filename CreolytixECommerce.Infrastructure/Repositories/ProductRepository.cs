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

        // Adds a new product to the database
        public async Task AddProductAsync(Product product)
        {
            await _productCollection.InsertOneAsync(product);
        }

        // Retrieves a product by ID
        public async Task<Product> GetByIdAsync(string productId)
        {
            return await _productCollection.Find(p => p.Id == productId).FirstOrDefaultAsync();
        }

        // Retrieves all products
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productCollection.Find(_ => true).ToListAsync();
        }

        // Retrieves products by category
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Category, category);
            return await _productCollection.Find(filter).ToListAsync();
        }

        // Updates an existing product
        public async Task UpdateProductAsync(Product product)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
            await _productCollection.ReplaceOneAsync(filter, product);
        }

        // Deletes a product by ID
        public async Task DeleteProductAsync(string productId)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, productId);
            await _productCollection.DeleteOneAsync(filter);
        }
    }
}

