using OnlineStore.Models.Enums;

namespace OnlineStore.Models.DTOs
{
    public class OrderSummaryDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatusEnum StatusEnum { get; set; }
        public string Email { get; set; } = string.Empty;
        public int ProductsNumber { get; set; }
        public int TotalQuantityItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
