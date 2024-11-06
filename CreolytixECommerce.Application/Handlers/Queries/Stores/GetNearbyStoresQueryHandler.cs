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
    public class GetNearbyStoresQueryHandler : IRequestHandler<GetNearbyStoresQuery, ResponseWrapper<IEnumerable<StoreDto>>>
    {
        private readonly IStoreRepository _storeRepository;

        public GetNearbyStoresQueryHandler(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<ResponseWrapper<IEnumerable<StoreDto>>> Handle(GetNearbyStoresQuery request, CancellationToken cancellationToken)
        {
            ResponseWrapper<IEnumerable<StoreDto>> response = new ResponseWrapper<IEnumerable<StoreDto>>();
            var stores = await _storeRepository.GetNearbyStoresAsync(request.Latitude, request.Longitude, request.Radius);

            response.IsSuccess = true;
            response.ResultDto = stores.Select(s => new StoreDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                Latitude = s.Location.Coordinates[1],
                Longitude = s.Location.Coordinates[0]
            });
            return response;
        }
    }
}
