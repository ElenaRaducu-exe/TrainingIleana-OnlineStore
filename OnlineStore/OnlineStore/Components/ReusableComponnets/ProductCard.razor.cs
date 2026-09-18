using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.Models;
using OnlineStore.Services.Contracts;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class ProductCard : ComponentBase
    {
        [Inject]
        private IBrandsService _brandsService {  get; set; }

        [Inject]
        private ICategoriesService _categoriesService { get; set; }

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
        public int CategoryId { get; set; }

        [Parameter]
        public int BrandId { get; set; }

        [Parameter]
        public EventCallback<int> OnSelectedId { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private Brand _brand { get; set; } = new();
        private Category _category { get; set; } = new(); 

        private async Task ProductCardClicked()
        {
            await OnSelectedId.InvokeAsync(ProductId);
        }

        protected override async Task OnInitializedAsync()
        {
            _brand = await _brandsService.GetBrandByIdAsync(BrandId);
            _category = await _categoriesService.GetCategoryByIdAsync(CategoryId);
        }
    }
}
