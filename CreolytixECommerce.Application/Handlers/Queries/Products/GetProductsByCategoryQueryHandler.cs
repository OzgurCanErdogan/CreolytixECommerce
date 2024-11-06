using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Queries.Products;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Handlers.Queries.Products
{
    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, ResponseWrapper<IEnumerable<ProductDto>>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsByCategoryQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ResponseWrapper<IEnumerable<ProductDto>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            ResponseWrapper<IEnumerable<ProductDto>> response = new ResponseWrapper<IEnumerable<ProductDto>>();
            // Retrieve products in the specified category
            var products = await _productRepository.GetProductsByCategoryAsync(request.Category);
            if(products == null)
            {
                response.IsSuccess = false;
                response.Message = "Products not found in this category";
                return response;
            }
            // Map each product to ProductDto
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price
            });
            response.IsSuccess = true;
            response.ResultDto = productDtos;
            return response;
        }
    }
}
