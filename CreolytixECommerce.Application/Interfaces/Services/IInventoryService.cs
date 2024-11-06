using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Interfaces.Services
{
    public interface IInventoryService
    {
        Task<bool> CheckProductAvailabilityAsync(string storeId, string productId, int quantity);
        Task UpdateInventoryAsync(string storeId, string productId, int quantity);
    }
}
