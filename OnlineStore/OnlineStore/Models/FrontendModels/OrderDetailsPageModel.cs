namespace OnlineStore.Models.FrontendModels
{
    public class OrderDetailsPageModel
    {
        public OrderDetailsModel? OrderDetails { get; set; } = new();
        public AddressModel? Address { get; set; } = new();
        public List<OrderItemDetailsModel>? OrderItems { get; set; } = new();
    }
}
