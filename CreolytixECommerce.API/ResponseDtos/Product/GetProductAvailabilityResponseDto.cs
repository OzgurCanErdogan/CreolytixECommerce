namespace CreolytixECommerce.API.ResponseDtos.Product
{
    public class GetProductAvailabilityResponseDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public int Quantity { get; set; }
        public double Distance { get; set; }
    }
}
