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
    public class GetProductAvailabilityQueryHandler : IRequestHandler<GetProductAvailabilityQuery, ResponseWrapper<IEnumerable<StoreDto>>>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public GetProductAvailabilityQueryHandler(IStoreRepository storeRepository, IInventoryRepository inventoryRepository)
        {
            _storeRepository = storeRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ResponseWrapper<IEnumerable<StoreDto>>> Handle(GetProductAvailabilityQuery request, CancellationToken cancellationToken)
        {
            ResponseWrapper<IEnumerable<StoreDto>> response = new ResponseWrapper<IEnumerable<StoreDto>>();
            // Retrieve nearby stores
            var nearbyStores = await _storeRepository.GetNearbyStoresAsync(request.Latitude, request.Longitude, request.Radius);
            if(nearbyStores == null)
            {
                response.IsSuccess = false;
                response.Message = "A problem occured while getting store information";
                return response;
            }

            var availableStores = new List<StoreDto>();

            foreach (var store in nearbyStores)
            {
                // Check inventory for product availability in each store
                var inventory = await _inventoryRepository.GetInventoryAsync(store.Id, request.ProductId);

                if (inventory != null && inventory.Quantity > 0)
                {
                    // Map store to StoreDto and include stock level
                    availableStores.Add(new StoreDto
                    {
                        Id = store.Id,
                        Name = store.Name,
                        Address = store.Address,
                        Latitude = store.Location.Coordinates[1],
                        Longitude = store.Location.Coordinates[0]
                    });
                }
            }
            response.IsSuccess = true;
            response.ResultDto = availableStores;
            return response;
        }
    }
}
