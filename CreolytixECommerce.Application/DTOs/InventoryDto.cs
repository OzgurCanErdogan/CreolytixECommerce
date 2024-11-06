using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.DTOs
{
    public class InventoryDto
    {
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
