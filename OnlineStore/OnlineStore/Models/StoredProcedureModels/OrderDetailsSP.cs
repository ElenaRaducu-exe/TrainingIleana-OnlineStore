namespace OnlineStore.Models.StoredProcedureModels
{
    public class OrderDetailsSP
    {
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int ProductsNumber { get; set; }
        public int TotalQuantityItems { get; set; }
        public decimal TotalPrice { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
