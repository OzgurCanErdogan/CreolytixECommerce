﻿using CreolytixECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Domain.Interfaces
{
    public interface IReservationRepository
    {
        Task CreateReservationAsync(Reservation reservation);
        Task<Reservation> GetReservationByIdAsync(string reservationId);
        Task UpdateReservationAsync(Reservation reservation);
        Task UpdateExpiredReservationStatusAsync(string reservationId);
    }
}
