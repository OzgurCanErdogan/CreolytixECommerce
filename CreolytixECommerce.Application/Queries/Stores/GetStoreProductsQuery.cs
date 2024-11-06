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
    public class GetStoreProductsQuery : IRequest<ResponseWrapper<StoreProductDto>>
    {
        public string StoreId { get; set; }
    }
}
