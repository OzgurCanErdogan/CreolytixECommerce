using CreolytixECommerce.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Interfaces.Services
{
    public interface IStoreService
    {
        Task<StoreDto> GetStoreByIdAsync(string storeId);
        Task<IEnumerable<StoreDto>> GetNearbyStoresAsync(double latitude, double longitude, double radius);
        //Task UpdateStoreDetailsAsync(StoreDto storeDto);
    }
}
