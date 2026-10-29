namespace OnlineStore.Models.DTOs
{
    public class ProductListSummaryDTO
    {
        public int TotalCount { get; set; }

        public List<ProductDTO> productDTOs { get; set; } = new List<ProductDTO>();
    }
}
