using OnlineStore.Models.Enums;

namespace OnlineStore.Models.FrontendModels
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int ProductsNumber { get; set; }
        public int TotalQuantityItems { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
