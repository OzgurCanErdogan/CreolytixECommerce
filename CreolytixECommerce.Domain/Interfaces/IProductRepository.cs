using CreolytixECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Domain.Interfaces
{
    public interface IProductRepository
    {
        // Adds a new product
        Task AddProductAsync(Product product);

        // Retrieves a product by ID
        Task<Product> GetByIdAsync(string productId);

        // Retrieves all products
        Task<IEnumerable<Product>> GetAllProductsAsync();

        // Retrieves products by category
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);

        // Updates an existing product
        Task UpdateProductAsync(Product product);

        // Deletes a product by ID
        Task DeleteProductAsync(string productId);
    }
}
