using Microsoft.AspNetCore.Components;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class CartItemCard : ComponentBase
    {
        [Parameter]
        public int Quantity { get; set; }
        [Parameter]
        public string ProductName { get; set; } = string.Empty;
        [Parameter]
        public string Description { get; set; } = string.Empty;
        [Parameter]
        public decimal Price { get; set; }
        [Parameter]
        public string ImageUrl { get; set; } = string.Empty;
        [Parameter]
        public string CategoryName { get; set; } = string.Empty;
        [Parameter]
        public string BrandName { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
