using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.Models;
using OnlineStore.Models.FrontendModels;
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
        public ProductModel ProductModel { get; set; }

        [Parameter]
        public EventCallback<int> OnSelectedId { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private Brand _brand { get; set; } = new();
        private Category _category { get; set; } = new(); 

        private async Task ProductCardClicked(int ProductId)
        {
            await OnSelectedId.InvokeAsync(ProductId);
        }

        protected override async Task OnInitializedAsync()
        {
            _brand = await _brandsService.GetBrandByIdAsync(ProductModel.BrandId);
            _category = await _categoriesService.GetCategoryByIdAsync(ProductModel.CategoryId);
        }
    }
}
