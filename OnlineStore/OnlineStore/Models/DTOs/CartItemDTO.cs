namespace OnlineStore.Models.DTOs
{
    public class CartItemDTO
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
    }
}
