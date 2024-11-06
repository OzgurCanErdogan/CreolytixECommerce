using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Queries.Stores
{
    public class GetNearbyStoresQuery : IRequest<ResponseWrapper<IEnumerable<StoreDto>>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
    }
}
