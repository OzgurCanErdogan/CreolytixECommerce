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
    public class GetAvailableStoresQueryHandler : IRequestHandler<GetAvailableStoresQuery, ResponseWrapper<IEnumerable<AvailableStoreDto>>>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IStoreRepository _storeRepository;

        public GetAvailableStoresQueryHandler(IInventoryRepository inventoryRepository, IStoreRepository storeRepository)
        {
            _inventoryRepository = inventoryRepository;
            _storeRepository = storeRepository;
        }

        public async Task<ResponseWrapper<IEnumerable<AvailableStoreDto>>> Handle(GetAvailableStoresQuery request, CancellationToken cancellationToken)
        {
            ResponseWrapper<IEnumerable<AvailableStoreDto>> response = new ResponseWrapper<IEnumerable<AvailableStoreDto>>();
            // Get inventory items for the specified product
            var inventoryItems = await _inventoryRepository.GetInventoryByProductIdAsync(request.ProductId);
            if (inventoryItems == null)
            {
                response.IsSuccess = false;
                response.Message = "There is no inventory information";
                return response;
            }

            var storeAvailability = new List<AvailableStoreDto>();

            foreach (var item in inventoryItems)
            {
                // Fetch the store details
                var store = await _storeRepository.GetByIdAsync(item.StoreId);
                if(store != null)
                {
                    // Calculate the distance from the given location
                    var distance = CalculateDistance(request.Latitude, request.Longitude, store.Location.Coordinates[1], store.Location.Coordinates[0]); //lat long

                    storeAvailability.Add(new AvailableStoreDto
                    {
                        StoreId = store.Id,
                        StoreName = store.Name,
                        Address = store.Address,
                        Quantity = item.Quantity,
                        Distance = distance
                    });
                }
            }

            // Return stores sorted by distance, closest first
            response.IsSuccess = true;
            response.ResultDto = storeAvailability.OrderBy(s => s.Distance).ToList();
            return response;
        }
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;

            double latDistance = ToRadians(lat2 - lat1);
            double lonDistance = ToRadians(lon2 - lon1);

            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private static double ToRadians(double angleInDegrees)
        {
            return angleInDegrees * (Math.PI / 180);
        }
    }
}
