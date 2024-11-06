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
    public class GetProductByIdQuery : IRequest<ResponseWrapper<ProductDto>>
    {
        public string ProductId { get; set; }
    }
}
