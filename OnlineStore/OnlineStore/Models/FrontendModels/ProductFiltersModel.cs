namespace OnlineStore.Models.FrontendModels
{
    public class ProductFiltersModel
    {
        public string? Name { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public bool? AvailableStock { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 9; 
    }
}
