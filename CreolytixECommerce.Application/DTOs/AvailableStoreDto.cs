using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.DTOs
{
    public class AvailableStoreDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public int Quantity { get; set; }
        public double Distance { get; set; }
    }
}
