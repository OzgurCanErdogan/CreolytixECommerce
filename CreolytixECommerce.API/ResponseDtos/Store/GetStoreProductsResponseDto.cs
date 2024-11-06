using CreolytixECommerce.Application.DTOs;

namespace CreolytixECommerce.API.ResponseDtos.Store
{
    public class GetStoreProductsResponseDto
    {
        public StoreProductResponseDto Store { get; set; }
        public List<ProductInventoryResponseDto> Products { get; set; }
    }
    public class StoreProductResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class ProductInventoryResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
