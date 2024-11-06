using CreolytixECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Domain.Interfaces
{
    public interface IStoreRepository
    {
        Task<Store> GetByIdAsync(string storeId);
        Task<IEnumerable<Store>> GetNearbyStoresAsync(double latitude, double longitude, double radius);
    }
}
