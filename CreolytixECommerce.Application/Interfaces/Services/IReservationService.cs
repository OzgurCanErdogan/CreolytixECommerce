using CreolytixECommerce.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Interfaces.Services
{
    public interface IReservationService
    {
        Task<string> CreateReservationAsync(ReservationDto reservation);
        Task<bool> CancelReservationAsync(string reservationId);
        Task<ReservationDto> GetReservationByIdAsync(string reservationId);
    }
}
