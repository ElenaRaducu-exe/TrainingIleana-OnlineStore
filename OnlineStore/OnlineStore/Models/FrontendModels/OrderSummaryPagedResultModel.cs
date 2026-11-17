namespace OnlineStore.Models.FrontendModels
{
    public class OrderSummaryPagedResultModel
    {
        public List<OrderSummaryModel> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
