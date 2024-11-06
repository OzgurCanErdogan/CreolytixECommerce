using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Interfaces.Services;
using CreolytixECommerce.Domain.Entities;
using CreolytixECommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<string> AddProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Category = productDto.Category,
                Price = productDto.Price
            };

            await _productRepository.AddProductAsync(product);
            return product.Id;
        }

        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price
            };
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(category);
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price
            });
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Category = productDto.Category,
                Price = productDto.Price
            };

            await _productRepository.UpdateProductAsync(product);
            return true;
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            await _productRepository.DeleteProductAsync(productId);
            return true;
        }
    }
}
