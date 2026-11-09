using OnlineStore.Models.StoredProcedureModels;

namespace OnlineStore.Models.FrontendModels
{
    public class CartSummaryPageModel
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }

        public decimal TotalOrderPrice { get; set; }
        public int TotalCartItems { get; set; }
        public int TotalItemsQuantity { get; set; }

        public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    }
}
