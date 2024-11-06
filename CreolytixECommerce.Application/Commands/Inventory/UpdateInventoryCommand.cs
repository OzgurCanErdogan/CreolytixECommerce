using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Commands.Inventory
{
    public class UpdateInventoryCommand : IRequest<ResponseWrapper<InventoryDto>>
    {
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; } // New quantity level
    }
}
