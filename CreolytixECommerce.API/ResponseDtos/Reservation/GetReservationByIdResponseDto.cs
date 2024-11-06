namespace CreolytixECommerce.API.ResponseDtos.Reservation
{
    public class GetReservationByIdResponseDto
    {
        public string Id { get; set; }
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public string Status { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
