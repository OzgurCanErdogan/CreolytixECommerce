using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Wrappers;
using MediatR;

namespace CreolytixECommerce.Application.Commands.Reservations
{
    public class CreateReservationCommand : IRequest<ResponseWrapper<ReservationDto>>
    {
        public string StoreId { get; set; }
        public string ProductId { get; set; }
    }
}
