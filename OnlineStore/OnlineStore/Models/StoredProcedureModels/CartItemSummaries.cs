namespace OnlineStore.Models.StoredProcedureModels
{
    public class CartItemSummaries
    {
        private int CartId { get; set; }
        private int UserId { get; set; }
        private int CartItemId { get; set; }
        private int Quantity { get; set; }
        private int ProductId { get; set; }
        private string Username { get; set; }
        private string ProductName { get; set; }
        private string Description { get; set; }
        private int Price { get; set; }
        private string ImageUrl { get; set; }
        private int CategoryId { get; set; }
        private int BrandId { get; set; }
    }
}
