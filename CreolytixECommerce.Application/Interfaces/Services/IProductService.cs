using CreolytixECommerce.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<string> AddProductAsync(ProductDto product);
        Task<ProductDto> GetProductByIdAsync(string productId);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
        Task<bool> UpdateProductAsync(ProductDto product);
        Task<bool> DeleteProductAsync(string productId);
    }
}
