using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OnlineStore.DBModels;
using OnlineStore.Models;
using OnlineStore.Services.Contracts;
using System.Net.NetworkInformation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OnlineStore.Components.Pages
{
    public partial class Products : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IBrandsService _brandService { get; set; }

        [Inject]
        private ICategoriesService _categoryService { get; set; }

        public List<ProductDTO> ProductsList = new(); 

        public int SelectedProductId { get; set; }

        private string _brand {  get; set; }
        private string _category {  get; set; }

        protected override async Task OnInitializedAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            ProductsList = await httpClient.GetFromJsonAsync<List<ProductDTO>>("api/admin/products");
        }

        protected async Task DeleteProduct(int? productId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            if (productId != null)
            {
                var product = ProductsList.FirstOrDefault(p => p.Id == productId);
                var response = await httpClient.DeleteAsync($"api/admin/products/delete/product/{productId}");

                if (response.IsSuccessStatusCode && product != null)
                {
                    ProductsList.Remove(product);
                    StateHasChanged();
                }
            }
        }

        protected async Task ChangeActiveMode(ProductDTO product)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            var response = await httpClient.PutAsJsonAsync($"api/admin/products/update/product/active-status/{product.Id}", product); 

            if (response.IsSuccessStatusCode)
            {
                var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDTO>(); 

                if(updatedProduct != null)
                {
                    var productList = ProductsList.FirstOrDefault(p => p.Id == product.Id);

                    if(productList != null)
                    {
                        productList.IsActive = updatedProduct.IsActive;
                    }

                    StateHasChanged();
                }
            }
        }

        protected void RedirectToEditForm(int productId)
        {
            _navigation.NavigateTo($"/dashboard/products/edit/{productId}");
        }

        protected void RedirectToProductDetailsPage(int id)
        {
            SelectedProductId = id;
            _navigation.NavigateTo($"/dashboard/products/{SelectedProductId}");
        }

        protected async Task<string> GetBrandName(int id)
        {
            return await _brandService.GetBrandNameByIdAsync(id); 
        }
    }
}
