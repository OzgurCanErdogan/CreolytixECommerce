using AutoMapper;
using CreolytixECommerce.API.RequestDtos.Reservation;
using CreolytixECommerce.API.ResponseDtos.Inventory;
using CreolytixECommerce.API.ResponseDtos.Product;
using CreolytixECommerce.API.ResponseDtos.Reservation;
using CreolytixECommerce.Application.Commands.Reservations;
using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Queries.Reservations;
using CreolytixECommerce.Application.Queries.Stores;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Domain.Entities;
using CreolytixECommerce.Infrastructure.Messaging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CreolytixECommerce.API.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageListener _messageListener;
        private readonly IMapper _mapper;

        public ReservationsController(IMessagePublisher messagePublisher, IMessageListener messageListener, IMapper mapper)
        {
            _messagePublisher = messagePublisher;
            _messageListener = messageListener;
            _mapper = mapper;
        }

        // Create a new reservation
        [HttpPost]
        public async Task<ActionResult<CreateReservationResponseDto>> CreateReservation(AddReservationDto reservationDto)
        {
            if (reservationDto == null)
            {
                return BadRequest("Invalid request data provided.");
            }

            var command = new CreateReservationCommand
            {
                StoreId = reservationDto.StoreId,
                ProductId = reservationDto.ProductId
            };
            var wrappedMessage = new MessageWrapper<CreateReservationCommand>(command);


            // Publish the create reservation command with correlation ID
            await _messagePublisher.PublishAsync("create_reservation_queue", wrappedMessage, wrappedMessage.CorrelationId);

            // Wait for response on the shared response queue with the matching correlation ID
            var response = await _messageListener.WaitForResponseAsync<ResponseWrapper<ReservationDto>>("reservation_response_queue", wrappedMessage.CorrelationId);

            var responseDto = _mapper.Map<CreateReservationResponseDto>(response.ResultDto);

            if (response.IsSuccess)
            {
                
                return Ok(responseDto);
            }
            else
            {
                return NotFound(response.Message);
            }
        }

        // Retrieve reservation details by ID
        [HttpGet("{reservationId}")]
        public async Task<ActionResult<GetReservationByIdResponseDto>> GetReservationById(string reservationId)
        {
            if (string.IsNullOrWhiteSpace(reservationId))
            {
                return BadRequest("Invalid request data provided.");
            }

            var query = new GetReservationByIdQuery { ReservationId = reservationId };
            var wrappedMessage = new MessageWrapper<GetReservationByIdQuery>(query);


            // Publish the query with correlation ID
            await _messagePublisher.PublishAsync("get_reservation_by_id_queue", wrappedMessage, wrappedMessage.CorrelationId);

            // Wait for response on the shared response queue with the matching correlation ID
            var response = await _messageListener.WaitForResponseAsync<ResponseWrapper<ReservationDto>>("reservation_response_queue", wrappedMessage.CorrelationId);

            var responseDto = _mapper.Map<GetReservationByIdResponseDto>(response.ResultDto);

            if (response.IsSuccess)
            {

                return Ok(responseDto);
            }
            else
            {
                return NotFound(response.Message);
            }
        }

        // Cancel an existing reservation by ID
        [HttpDelete("{reservationId}")]
        public async Task<IActionResult> CancelReservation(string reservationId)
        {
            if (string.IsNullOrWhiteSpace(reservationId))
            {
                return BadRequest("Invalid request data provided.");
            }

            var command = new CancelReservationCommand { ReservationId = reservationId };
            var wrappedMessage = new MessageWrapper<CancelReservationCommand>(command);


            // Publish the cancel reservation command with correlation ID
            await _messagePublisher.PublishAsync("cancel_reservation_queue", wrappedMessage, wrappedMessage.CorrelationId);

            // Wait for response on the shared response queue with the matching correlation ID
            var result = await _messageListener.WaitForResponseAsync<ResponseWrapper<bool>>("reservation_response_queue", wrappedMessage.CorrelationId);

            if (result.IsSuccess)
            {

                return Ok(result.ResultDto);
            }
            else
            {
                return NotFound(result.Message);
            }
        }
    }
}
