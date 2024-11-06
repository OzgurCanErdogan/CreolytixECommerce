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
    public class GetStoreByIdQueryHandler : IRequestHandler<GetStoreByIdQuery, ResponseWrapper<StoreDto>>
    {
        private readonly IStoreRepository _storeRepository;

        public GetStoreByIdQueryHandler(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<ResponseWrapper<StoreDto>> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken)
        {
            ResponseWrapper<StoreDto> response = new ResponseWrapper<StoreDto>();
            // Retrieve the store by ID from the repository
            var store = await _storeRepository.GetByIdAsync(request.StoreId);

            // If the store is not found, return null
            if (store == null)
            {
                response.IsSuccess = false;
                response.Message = "Store is not found";
                return response;
            }

            // Map the store entity to StoreDto
            response.IsSuccess = true;
            response.ResultDto = new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Latitude = store.Location.Coordinates[1],
                Longitude = store.Location.Coordinates[0]
            };
            return response;
        }
    }
}
