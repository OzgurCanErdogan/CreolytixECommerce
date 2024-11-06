using CreolytixECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        Task<Inventory> GetInventoryAsync(string storeId, string productId);
        Task UpdateInventoryAsync(Inventory inventory);
        Task<IEnumerable<Inventory>> GetInventoryByStoreIdAsync(string storeId);
        Task<IEnumerable<Inventory>> GetInventoryByProductIdAsync(string productId);
        Task CreateInventoryAsync(Inventory inventory);

    }
}
