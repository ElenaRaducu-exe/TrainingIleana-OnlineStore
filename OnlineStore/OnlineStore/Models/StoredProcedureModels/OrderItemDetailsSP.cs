namespace OnlineStore.Models.StoredProcedureModels
{
    public class OrderItemDetailsSP
    {
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}
