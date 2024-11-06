using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Interfaces.Services;
using CreolytixECommerce.Domain.Entities;
using CreolytixECommerce.Domain.Enums;
using CreolytixECommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IInventoryService _inventoryService;

        public ReservationService(IReservationRepository reservationRepository, IInventoryService inventoryService)
        {
            _reservationRepository = reservationRepository;
            _inventoryService = inventoryService;
        }

        // Create a new reservation and update inventory
        public async Task<string> CreateReservationAsync(ReservationDto reservationDto)
        {
            // Check product availability
            bool isAvailable = await _inventoryService.CheckProductAvailabilityAsync(
                reservationDto.StoreId,
                reservationDto.ProductId,
                reservationDto.Quantity);

            if (!isAvailable) return null;

            // Create a new reservation entity
            var reservation = new Reservation
            {
                StoreId = reservationDto.StoreId,
                ProductId = reservationDto.ProductId,
                Quantity = reservationDto.Quantity,
                Status = ReservationStatus.Active,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // 24-hour expiry
            };

            // Save reservation and update inventory
            await _reservationRepository.CreateReservationAsync(reservation);
            await _inventoryService.UpdateInventoryAsync(reservation.StoreId, reservation.ProductId, reservationDto.Quantity);

            return reservation.Id;
        }

        // Cancel an existing reservation and update inventory
        public async Task<bool> CancelReservationAsync(string reservationId)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
            if (reservation == null || reservation.Status != ReservationStatus.Active) return false;

            reservation.Status = ReservationStatus.Canceled;
            await _reservationRepository.UpdateReservationAsync(reservation);

            // Restore inventory
            var updatedQuantity = reservation.Quantity; // Quantity to be added back to inventory
            await _inventoryService.UpdateInventoryAsync(reservation.StoreId, reservation.ProductId, updatedQuantity);

            return true;
        }

        // Retrieve a reservation by ID
        public async Task<ReservationDto> GetReservationByIdAsync(string reservationId)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
            if (reservation == null) return null;

            return new ReservationDto
            {
                Id = reservation.Id,
                StoreId = reservation.StoreId,
                ProductId = reservation.ProductId,
                Quantity = reservation.Quantity,
                Status = reservation.Status.ToString(),
                ExpiresAt = reservation.ExpiresAt
            };
        }
    }
}
