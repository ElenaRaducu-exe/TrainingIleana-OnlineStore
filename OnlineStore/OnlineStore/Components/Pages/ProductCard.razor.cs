using Microsoft.AspNetCore.Components;
using OnlineStore.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Components.Pages
{
    public partial class ProductCard : ComponentBase
    {
        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string? Description { get; set; }

        [Parameter]
        public decimal Price { get; set; }

        [Parameter]
        public int Stock { get; set; }

        [Parameter]
        public string? ImageUrl { get; set; }

        [Parameter]
        public bool IsActive { get; set; }

        [Parameter]
        public string Category { get; set; }

        [Parameter]
        public string Brand { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; } 
    }
}
