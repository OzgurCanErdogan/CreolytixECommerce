using CreolytixECommerce.Application.Interfaces.Services;
using CreolytixECommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        // Check if the required quantity is available in the specified store's inventory
        public async Task<bool> CheckProductAvailabilityAsync(string storeId, string productId, int quantity)
        {
            var inventory = await _inventoryRepository.GetInventoryAsync(storeId, productId);
            return inventory != null && inventory.Quantity >= quantity;
        }

        // Update the inventory with a new quantity level
        public async Task UpdateInventoryAsync(string storeId, string productId, int quantity)
        {
            var inventory = await _inventoryRepository.GetInventoryAsync(storeId, productId);
            if (inventory != null)
            {
                inventory.Quantity = quantity;
                await _inventoryRepository.UpdateInventoryAsync(inventory);
            }
        }
    }
}
