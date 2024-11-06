using CreolytixECommerce.Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Commands.Reservations
{
    public class CancelReservationCommand : IRequest<ResponseWrapper<bool>>
    {
        public string ReservationId { get; set; }
    }
}
