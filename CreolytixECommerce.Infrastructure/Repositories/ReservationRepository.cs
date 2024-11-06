using CreolytixECommerce.Domain.Entities;
using CreolytixECommerce.Domain.Interfaces;
using CreolytixECommerce.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly IMongoCollection<Reservation> _reservationCollection;

        public ReservationRepository(MongoDbContext context)
        {
            _reservationCollection = context.GetCollection<Reservation>("reservations");
        }

        // Create a new reservation
        public async Task CreateReservationAsync(Reservation reservation)
        {
            await _reservationCollection.InsertOneAsync(reservation);
        }

        // Retrieve a reservation by ID
        public async Task<Reservation> GetReservationByIdAsync(string reservationId)
        {
            return await _reservationCollection
                .Find(res => res.Id == reservationId)
                .FirstOrDefaultAsync();
        }

        // Update an existing reservation
        public async Task UpdateReservationAsync(Reservation reservation)
        {
            var filter = Builders<Reservation>.Filter.Eq(r => r.Id, reservation.Id);
            await _reservationCollection.ReplaceOneAsync(filter, reservation);
        }

        // Update expired reservation's status
        public async Task UpdateExpiredReservationStatusAsync(string reservationId)
        {
            var reservation = await _reservationCollection
                .Find(res => res.Id == reservationId)
                .FirstOrDefaultAsync();

            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation with ID {reservationId} not found.");
            }

            if (DateTime.UtcNow > reservation.ExpiresAt)
            {
                
                reservation.Status = Domain.Enums.ReservationStatus.Expired;

                var filter = Builders<Reservation>.Filter.Eq(res => res.Id, reservationId);
                await _reservationCollection.ReplaceOneAsync(filter, reservation);
            }
        }
    }
}
