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
        Task<Product> GetByIdAsync(string productId);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    }
}
