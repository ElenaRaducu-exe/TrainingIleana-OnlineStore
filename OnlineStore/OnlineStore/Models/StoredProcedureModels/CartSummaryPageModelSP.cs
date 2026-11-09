namespace OnlineStore.Models.StoredProcedureModels
{
    public class CartSummaryPageModelSP
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }

        public decimal TotalOrderPrice { get; set; }
        public int TotalCartItems { get; set; }
        public int TotalItemsQuantity { get; set; }

        public List<CartItemSummaries> CartItems { get; set; } = new List<CartItemSummaries>();
    }
}
