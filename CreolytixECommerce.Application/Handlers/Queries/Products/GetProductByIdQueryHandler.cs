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
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ResponseWrapper<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ResponseWrapper<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            ResponseWrapper<ProductDto> response = new ResponseWrapper<ProductDto>();
            // Retrieve the product from the repository
            var product = await _productRepository.GetByIdAsync(request.ProductId);

            // If product is not found, return null
            if (product == null)
            {
                response.IsSuccess = false;
                response.Message = "Product is not found";
                return response;
            }

            // Map product to ProductDto
            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price
            };
            response.IsSuccess = true;
            response.ResultDto = productDto;
            return response;
        }
    }
}
