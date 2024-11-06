using CreolytixECommerce.Application.Commands.Inventory;
using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Handlers.Commands.Inventory
{
    public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, ResponseWrapper<InventoryDto>>
    {
        private readonly IInventoryRepository _inventoryRepository;

        public UpdateInventoryCommandHandler(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ResponseWrapper<InventoryDto>> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the current inventory for the specified product and store
            var inventory = await _inventoryRepository.GetInventoryAsync(request.StoreId, request.ProductId);

            if (inventory == null)
            {
                inventory = new Domain.Entities.Inventory()
                {
                    StoreId = request.StoreId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await _inventoryRepository.CreateInventoryAsync(inventory);
            }
            else
            {
                inventory.Quantity = request.Quantity;
                await _inventoryRepository.UpdateInventoryAsync(inventory);
            }

            InventoryDto result = new InventoryDto()
            {
                StoreId = inventory.StoreId,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity
            };

            ResponseWrapper<InventoryDto> response = new ResponseWrapper<InventoryDto>();
            response.IsSuccess = true;
            response.ResultDto = result;

            return response;
        }
    }
}
