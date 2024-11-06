using CreolytixECommerce.Application.Commands.Reservations;
using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Interfaces;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Domain.Entities;
using CreolytixECommerce.Domain.Enums;
using CreolytixECommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Handlers.Commands.Reservations
{
    public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ResponseWrapper<ReservationDto>>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public CreateReservationCommandHandler(IReservationRepository reservationRepository, IInventoryRepository inventoryRepository)
        {
            _reservationRepository = reservationRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ResponseWrapper<ReservationDto>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            ResponseWrapper<ReservationDto> response = new ResponseWrapper<ReservationDto>();
            // Check inventory for product availability
            var inventory = await _inventoryRepository.GetInventoryAsync(request.StoreId, request.ProductId);
            if (inventory == null || inventory.Quantity < 1)
            {
                response.IsSuccess = false;
                response.Message = "Insufficient inventory item";
                return response; // Insufficient inventory
            }

            // Create new reservation
            var reservation = new Reservation
            {
                StoreId = request.StoreId,
                ProductId = request.ProductId,
                //Quantity = request.Quantity,
                Status = ReservationStatus.Active,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // 24-hour expiry
            };

            // Update inventory
            inventory.Quantity = inventory.Quantity -1;
            await _inventoryRepository.UpdateInventoryAsync(inventory);

            // Save reservation
            await _reservationRepository.CreateReservationAsync(reservation);
            ReservationDto reservationDto = new ReservationDto()
            {
                Id = reservation.Id,
                StoreId = reservation.StoreId,
                ProductId = reservation.ProductId,
                ExpiresAt = reservation.ExpiresAt,
                Status = reservation.Status == ReservationStatus.Active ? "Active" : reservation.Status == ReservationStatus.Canceled ? "Canceled" : "Expired"
            };
            response.IsSuccess = true;
            response.ResultDto = reservationDto;

            return response;
        }
    }
}
