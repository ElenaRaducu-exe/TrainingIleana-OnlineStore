namespace OnlineStore.Models.DTOs
{
    public class OrderDetailsPageDTO
    {
        public OrderDetailsDTO? OrderDetails { get; set; } = new(); 
        public AddressDTO? Address { get; set; } = new();
        public List<OrderItemDetailsDTO>? OrderItems { get; set; } = new();
    }
}
