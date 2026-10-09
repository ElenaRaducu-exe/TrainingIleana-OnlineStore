namespace OnlineStore.Models.DTOs
{
    public class CreateOrderRequestDTO
    {
        public int? AddressId { get; set; }
        public int? CartId { get; set; }
        public int? UserId { get; set; }
    }
}
