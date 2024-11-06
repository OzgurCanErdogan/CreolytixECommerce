using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.DTOs
{
    public class StoreProductDto
    {
        public StoreDto Store { get; set; }
        public List<ProductInventoryDto> Products { get; set; }
    }

    public class ProductInventoryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
