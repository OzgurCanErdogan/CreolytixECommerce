using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Queries.Reservations
{
    public class GetReservationByIdQuery : IRequest<ResponseWrapper<ReservationDto>>
    {
        public string ReservationId { get; set; }
    }
}
