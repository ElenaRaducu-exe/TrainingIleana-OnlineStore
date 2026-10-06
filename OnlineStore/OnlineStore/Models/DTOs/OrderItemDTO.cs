namespace OnlineStore.Models.DTOs
{
    public class OrderItemDTO
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}
