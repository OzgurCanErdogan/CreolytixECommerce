using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.DTOs
{
    public class ReservationDto
    {
        public string Id { get; set; }
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public string Status { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
