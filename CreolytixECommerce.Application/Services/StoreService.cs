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
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<StoreDto> GetStoreByIdAsync(string storeId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null) return null;

            return new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Latitude = store.Location.Coordinates[0],
                Longitude = store.Location.Coordinates[1]
            };
        }

        public async Task<IEnumerable<StoreDto>> GetNearbyStoresAsync(double latitude, double longitude, double radius)
        {
            var stores = await _storeRepository.GetNearbyStoresAsync(latitude, longitude, radius);
            return stores.Select(s => new StoreDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                Latitude = s.Location.Coordinates[0],
                Longitude = s.Location.Coordinates[1]
            });
        }
        /*
        public async Task UpdateStoreDetailsAsync(StoreDto storeDto)
        {
            var store = new Store
            {
                Id = storeDto.Id,
                Name = storeDto.Name,
                Address = storeDto.Address,
                Location = new Location
                {
                    Latitude = storeDto.Latitude,
                    Longitude = storeDto.Longitude
                }
            };

            await _storeRepository.UpdateStoreAsync(store);
        }
        */
    }
}
