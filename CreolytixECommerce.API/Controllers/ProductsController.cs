using AutoMapper;
using CreolytixECommerce.API.RequestDtos;
using CreolytixECommerce.API.ResponseDtos.Inventory;
using CreolytixECommerce.API.ResponseDtos.Product;
using CreolytixECommerce.Application.DTOs;
using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Queries.Products;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace CreolytixECommerce.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageListener _messageListener;
        private readonly IMapper _mapper;

        public ProductsController(IMessagePublisher messagePublisher, IMessageListener messageListener, IMapper mapper)
        {
            _messagePublisher = messagePublisher;
            _messageListener = messageListener;
            _mapper = mapper;
        }

        
        // Retrieve product details by ID
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductDto>> GetProductById(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return BadRequest("Invalid request data provided.");
            }

            var query = new GetProductByIdQuery { ProductId = productId };
            var wrappedQuery = new MessageWrapper<GetProductByIdQuery>(query);

            // Publish the wrapped query with the CorrelationId
            await _messagePublisher.PublishAsync("get_product_by_id_queue", wrappedQuery, "product_response_queue");

            // Wait for response with the matching CorrelationId
            var product = await _messageListener.WaitForResponseAsync<ResponseWrapper<ProductDto>>("product_response_queue", wrappedQuery.CorrelationId);

            var responseDto = _mapper.Map<GetProductByIdResponseDto>(product.ResultDto);

            if (product.IsSuccess)
            {
                return Ok(responseDto);
            }
            else
            {
                return NotFound(product.Message);
            }
        }

        // Retrieve products by category
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<GetProductsByCategoryResponseDto>>> GetProductsByCategory([FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Invalid request data provided.");
            }

            var query = new GetProductsByCategoryQuery { Category = category };
            var wrappedQuery = new MessageWrapper<GetProductsByCategoryQuery>(query);

            // Publish the wrapped query with the CorrelationId
            await _messagePublisher.PublishAsync("get_products_by_category_queue", wrappedQuery, "product_response_queue");

            // Wait for response with the matching CorrelationId
            var products = await _messageListener.WaitForResponseAsync<ResponseWrapper<IEnumerable<ProductDto>>>("product_response_queue", wrappedQuery.CorrelationId);

            if (products.IsSuccess)
            {
                var responseDto = _mapper.Map<List<GetProductsByCategoryResponseDto>>(products.ResultDto);
                return Ok(responseDto);
            }
            else
            {
                return NotFound(products.Message);
            }
        }

        [HttpGet("{productId}/availability")]
        public async Task<ActionResult<IEnumerable<AvailableStoreDto>>> GetProductAvailability(string productId, [FromQuery] double lat, [FromQuery] double lng)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return BadRequest("Invalid request data provided.");
            }

            var query = new GetAvailableStoresQuery { 
                ProductId = productId,
                Latitude = lat,
                Longitude = lng
            };
            var wrappedQuery = new MessageWrapper<GetAvailableStoresQuery>(query);

            // Publish the wrapped query with the CorrelationId
            await _messagePublisher.PublishAsync("get_product_available_store_queue", wrappedQuery, "product_response_queue");

            // Wait for response with the matching CorrelationId
            var stores = await _messageListener.WaitForResponseAsync<ResponseWrapper<IEnumerable<AvailableStoreDto>>>("product_response_queue", wrappedQuery.CorrelationId);
            var responseDto = _mapper.Map<List<GetProductAvailabilityResponseDto>>(stores.ResultDto);

            if (stores.IsSuccess)
            {
                return Ok(responseDto);
            }
            else
            {
                return NotFound(stores.Message);
            }

        }
    }
}
