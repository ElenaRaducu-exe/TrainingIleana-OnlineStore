namespace OnlineStore.Models.DTOs
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
    }
}
