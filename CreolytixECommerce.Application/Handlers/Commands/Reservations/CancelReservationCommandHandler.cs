using CreolytixECommerce.Application.Commands.Reservations;
using CreolytixECommerce.Application.Wrappers;
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
    public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, ResponseWrapper<bool>>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public CancelReservationCommandHandler(
            IReservationRepository reservationRepository,
            IInventoryRepository inventoryRepository)
        {
            _reservationRepository = reservationRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ResponseWrapper<bool>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            ResponseWrapper<bool> response = new ResponseWrapper<bool>();
            // Retrieve the reservation
            var reservation = await _reservationRepository.GetReservationByIdAsync(request.ReservationId);

            // If reservation is not found or is already canceled, return false
            if (reservation == null)
            {
                response.IsSuccess = false;
                response.ResultDto = false;
                response.Message = "Reservation is not found";
                return response;
            }
            
            if(reservation.Status != ReservationStatus.Active)
            {
                response.IsSuccess = false;
                response.ResultDto = true;
                response.Message = "Reservation is already canceled";
                return response;

            }

            // Update reservation status to Canceled
            reservation.Status = ReservationStatus.Canceled;
            await _reservationRepository.UpdateReservationAsync(reservation);

            // Restore inventory for the reserved product
            var inventory = await _inventoryRepository.GetInventoryAsync(reservation.StoreId, reservation.ProductId);
            if (inventory != null)
            {
                inventory.Quantity = inventory.Quantity + 1;
                await _inventoryRepository.UpdateInventoryAsync(inventory);
            }
            response.IsSuccess = true;
            response.ResultDto = true;
            return response;
        }
    }
}
