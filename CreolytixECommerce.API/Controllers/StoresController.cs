using AutoMapper;
using CreolytixECommerce.API.ResponseDtos.Product;
using CreolytixECommerce.API.ResponseDtos.Store;
using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Queries.Stores;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CreolytixECommerce.API.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageListener _messageListener;
        private readonly IMapper _mapper;

        public StoresController(IMessagePublisher messagePublisher, IMessageListener messageListener, IMapper mapper)
        {
            _messagePublisher = messagePublisher;
            _messageListener = messageListener;
            _mapper = mapper;
        }

        // Retrieve store details by ID
        [HttpGet("{storeId}")]
        public async Task<ActionResult<GetStoreByIdResponseDto>> GetStoreById(string storeId)
        {
            if (string.IsNullOrWhiteSpace(storeId))
            {
                return BadRequest("Invalid request data provided.");
            }

            var query = new GetStoreByIdQuery { StoreId = storeId };
            var wrappedQuery = new MessageWrapper<GetStoreByIdQuery>(query);

            // Publish the wrapped query to the get_store_by_id_queue
            await _messagePublisher.PublishAsync("get_store_by_id_queue", wrappedQuery, "store_response_queue");

            // Wait for response with the matching CorrelationId
            var store = await _messageListener.WaitForResponseAsync<ResponseWrapper<StoreDto>>("store_response_queue", wrappedQuery.CorrelationId);
            var responseDto = _mapper.Map<GetStoreByIdResponseDto>(store.ResultDto);

            if (store.IsSuccess)
            {

                return Ok(responseDto);
            }
            else
            {
                return NotFound(store.Message);
            }
        }

        // Find nearby stores within a certain radius
        [HttpGet("nearby")]
        public async Task<ActionResult<IEnumerable<GetNearbyStoresResponseDto>>> GetNearbyStores(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radius)
        {
            var query = new GetNearbyStoresQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                Radius = radius
            };
            var wrappedQuery = new MessageWrapper<GetNearbyStoresQuery>(query);

            // Publish the wrapped query to the get_nearby_stores_queue
            await _messagePublisher.PublishAsync("get_nearby_stores_queue", wrappedQuery, "store_response_queue");

            // Wait for response with the matching CorrelationId
            var stores = await _messageListener.WaitForResponseAsync<ResponseWrapper<IEnumerable<StoreDto>>>("store_response_queue", wrappedQuery.CorrelationId);
            var responseDto = _mapper.Map<List<GetNearbyStoresResponseDto>>(stores.ResultDto);

            if (stores.IsSuccess)
            {

                return Ok(responseDto);
            }
            else
            {
                return NotFound(stores.Message);
            }
        }

        [HttpGet("{storeId}/products")]
        public async Task<ActionResult<IEnumerable<GetStoreProductsResponseDto>>> GetStoreProducts(string storeId)
        {
            if (string.IsNullOrWhiteSpace(storeId))
            {
                return BadRequest("Invalid request data provided.");
            }

            var query = new GetStoreProductsQuery { StoreId = storeId };
            var wrappedQuery = new MessageWrapper<GetStoreProductsQuery>(query);

            // Publish the wrapped query to the get_products_store_by_id_queue
            await _messagePublisher.PublishAsync("get_products_store_by_id_queue", wrappedQuery, "store_response_queue");

            // Wait for response with the matching CorrelationId
            var store = await _messageListener.WaitForResponseAsync<ResponseWrapper<StoreProductDto>>("store_response_queue", wrappedQuery.CorrelationId);
            var responseDto = _mapper.Map<List<GetStoreProductsResponseDto>>(store.ResultDto);

            if (store.IsSuccess)
            {

                return Ok(responseDto);
            }
            else
            {
                return NotFound(store.Message);
            }
        }
    }
}
