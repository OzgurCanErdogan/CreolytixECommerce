namespace CreolytixECommerce.API.ResponseDtos.Product
{
    public class GetProductsByCategoryResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}
