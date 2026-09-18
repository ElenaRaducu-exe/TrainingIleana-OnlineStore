using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
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

        protected async Task DeleteProduct(int productId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            var ProductsList = await httpClient.GetFromJsonAsync<List<ProductDTO>>("api/admin/products");

            var product = ProductsList.FirstOrDefault(p => p.Id == productId);
            var response = await httpClient.DeleteAsync($"api/admin/products/delete/product/{productId}");

            if (response.IsSuccessStatusCode && product != null)
            {
                ProductsList.Remove(product);
                StateHasChanged();
                _navigation.NavigateTo("/dashboard/products");
            }
        }

        protected void EditProduct(int productId)
        {
            _navigation.NavigateTo($"/dashboard/products/edit/{productId}");
        }

        protected async Task AddToCart(ProductDTO productDTO)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            productDTO.Stock--;

            var response = await httpClient.PutAsJsonAsync($"api/admin/products/update/product/{productDTO.Id}", productDTO);

            StateHasChanged();
        }
    }
}
