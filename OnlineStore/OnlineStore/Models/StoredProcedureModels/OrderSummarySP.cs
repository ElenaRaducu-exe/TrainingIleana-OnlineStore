namespace OnlineStore.Models.StoredProcedureModels
{
    public class OrderSummarySP
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int StatusId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int ProductsNumber { get; set; }
        public int TotalQuantityItems { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalCount { get; set; }
    }
}
