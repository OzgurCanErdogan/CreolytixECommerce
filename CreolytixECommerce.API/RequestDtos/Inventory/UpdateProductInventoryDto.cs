namespace CreolytixECommerce.API.RequestDtos.Inventory
{
    public class UpdateProductInventoryDto
    {
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
