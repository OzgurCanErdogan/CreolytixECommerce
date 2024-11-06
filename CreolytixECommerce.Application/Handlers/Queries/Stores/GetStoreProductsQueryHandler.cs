using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Queries.Stores;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Handlers.Queries.Stores
{
    public class GetStoreProductsQueryHandler : IRequestHandler<GetStoreProductsQuery, ResponseWrapper<StoreProductDto>>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;

        public GetStoreProductsQueryHandler(IInventoryRepository inventoryRepository, IProductRepository productRepository, IStoreRepository storeRepository)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
        }

        public async Task<ResponseWrapper<StoreProductDto>> Handle(GetStoreProductsQuery request, CancellationToken cancellationToken)
        {
            ResponseWrapper<StoreProductDto> response = new ResponseWrapper<StoreProductDto>();
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if(store == null)
            {
                response.IsSuccess = false;
                response.Message = "Store not found";
                return response;
            }
            // Get inventory items for the specified store
            var inventoryItems = await _inventoryRepository.GetInventoryByStoreIdAsync(request.StoreId);
            if (inventoryItems == null)
            {
                response.IsSuccess = false;
                response.Message = "There is no inventory information";
                return response;
            }


            // Retrieve product details and map to DTO
            var result = new StoreProductDto();
            result.Store = new StoreDto()
            {
                Id = store.Id,
                Address = store.Address,
                Latitude = store.Location.Coordinates[1],
                Longitude = store.Location.Coordinates[0],
                Name = store.Name
            };
            result.Products = new List<ProductInventoryDto>();
            foreach (var item in inventoryItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if(product != null)
                {
                    result.Products.Add(new ProductInventoryDto
                    {
                        Id = item.ProductId,
                        Category = product.Category,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = item.Quantity
                    });
                }
                
            }
            response.IsSuccess = true;
            response.ResultDto = result;
            return response;
        }
    }
}
