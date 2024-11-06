using AutoMapper;
using CreolytixECommerce.API.RequestDtos.Inventory;
using CreolytixECommerce.API.RequestDtos.Reservation;
using CreolytixECommerce.API.ResponseDtos.Inventory;
using CreolytixECommerce.Application.Commands.Inventory;
using CreolytixECommerce.Application.Commands.Reservations;
using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Wrappers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CreolytixECommerce.API.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageListener _messageListener;
        private readonly IMapper _mapper;
        public InventoryController(IMessagePublisher messagePublisher, IMessageListener messageListener, IMapper mapper)
        {
            _messagePublisher = messagePublisher;
            _messageListener = messageListener;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<UpdateInventoryResponseDto>> UpdateInventory(UpdateProductInventoryDto updateProductInventoryDto)
        {
            if(updateProductInventoryDto == null)
            {
                return BadRequest("Invalid request data provided.");
            }

            var command = new UpdateInventoryCommand
            {
                StoreId = updateProductInventoryDto.StoreId,
                ProductId = updateProductInventoryDto.ProductId,
                Quantity = updateProductInventoryDto.Quantity
            };
            var wrappedMessage = new MessageWrapper<UpdateInventoryCommand>(command);


            // Publish the create reservation command with correlation ID
            await _messagePublisher.PublishAsync("update_inventory_queue", wrappedMessage, wrappedMessage.CorrelationId);

            // Wait for response on the shared response queue with the matching correlation ID
            var response = await _messageListener.WaitForResponseAsync<ResponseWrapper<InventoryDto>>("inventory_response_queue", wrappedMessage.CorrelationId);

            var responseDto = _mapper.Map<UpdateInventoryResponseDto>(response.ResultDto);
            if (response.IsSuccess)
            {
                return Ok(responseDto);
            }
            else
            {
                return NotFound(response.Message);
            }
        }
    }
}
