namespace OnlineStore.Models.DTOs
{
    public class OrderSummaryPagedResultDTO
    {
        public List<OrderSummaryDTO> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
