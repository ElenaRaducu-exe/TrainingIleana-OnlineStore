namespace OnlineStore.Models.FrontendModels
{
    public class CartItemModel
    {
        public int Quantity { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
    }
}
