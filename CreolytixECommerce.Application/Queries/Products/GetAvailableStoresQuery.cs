using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Queries.Products
{
    public class GetAvailableStoresQuery : IRequest<ResponseWrapper<IEnumerable<AvailableStoreDto>>>
    {
        public string ProductId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
