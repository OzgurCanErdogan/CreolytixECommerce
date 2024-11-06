using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Queries.Reservations;
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

namespace CreolytixECommerce.Application.Handlers.Queries.Reservations
{
    public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ResponseWrapper<ReservationDto>>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IInventoryRepository _inventoryRepository;
        public GetReservationByIdQueryHandler(IReservationRepository reservationRepository, IInventoryRepository inventoryRepository)
        {
            _reservationRepository = reservationRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ResponseWrapper<ReservationDto>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
        {
            ResponseWrapper<ReservationDto> response = new ResponseWrapper<ReservationDto>();
            await _reservationRepository.UpdateExpiredReservationStatusAsync(request.ReservationId);
            // Retrieve the reservation from the repository
            var reservation = await _reservationRepository.GetReservationByIdAsync(request.ReservationId);

            // If the reservation is not found, return null
            if (reservation == null)
            {
                response.IsSuccess = false;
                response.Message = "The reservation is not found";
                return response;
            }

            if(reservation.Status != ReservationStatus.Active)
            {
                var inventory = await _inventoryRepository.GetInventoryAsync(reservation.StoreId, reservation.ProductId);
                if (inventory != null)
                {
                    inventory.Quantity = inventory.Quantity + 1;
                    await _inventoryRepository.UpdateInventoryAsync(inventory);
                }
            }

            // Map reservation to ReservationDto
            var reservationDto = new ReservationDto
            {
                Id = reservation.Id,
                StoreId = reservation.StoreId,
                ProductId = reservation.ProductId,
                Status = reservation.Status.ToString(),
                ExpiresAt = reservation.ExpiresAt
            };
            response.IsSuccess = true;
            response.ResultDto = reservationDto;
            return response;
        }
    }
}
