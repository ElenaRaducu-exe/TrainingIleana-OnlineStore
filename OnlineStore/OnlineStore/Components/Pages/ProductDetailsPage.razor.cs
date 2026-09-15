using Microsoft.AspNetCore.Components;
using OnlineStore.Models;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class ProductDetailsPage : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IBrandsService _brandService { get; set; }

        [Inject]
        private ICategoriesService _categoryService { get; set; }

        [Parameter]
        public int? Id { get; set; }

        private string _brand { get; set; }
        private string _category { get; set; }

        protected ProductDTO productDTO { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            productDTO = await httpClient.GetFromJsonAsync<ProductDTO>($"api/admin/products/get/product/{Id}");

            _brand = await _brandService.GetBrandNameByIdAsync(productDTO.BrandId);
            _category = await _categoryService.GetCategoryNameByIdAsync(productDTO.CategoryId);
        }

        protected void DeleteProduct()
        {
            Console.WriteLine("------------ DeleteProduct");
        }
        protected void EditProduct()
        {
            Console.WriteLine("------------ EditProduct");
        }
    }
}
