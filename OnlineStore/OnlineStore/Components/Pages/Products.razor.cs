using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using System.Net.Http.Headers;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using AutoMapper;

namespace OnlineStore.Components.Pages
{
    public partial class Products : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private ITokenProvider _tokenProvider { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        public List<ProductModel> ProductsList = new(); 

        public int SelectedProductId { get; set; }

        private bool _resultAddProductToCart { get; set; } = false;

        private int _pageNumber = 1; 
        private int _pageSize = 20;

        protected async Task LoadProducts()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            var productDTOs = await httpClient.GetFromJsonAsync<List<ProductDTO>>("api/admin/products");

            ProductsList = _mapper.Map<List<ProductModel>>(productDTOs);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts(); 

            _resultAddProductToCart = false; 
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

        protected async Task ChangeActiveMode(ProductModel product)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            var productDTO = _mapper.Map<ProductModel>(product);
            var response = await httpClient.PutAsJsonAsync($"api/admin/products/update/product/active-status/{product.Id}", productDTO); 

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

        protected async Task AddToCart(ProductModel product)
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            product.Stock--;

            StateHasChanged();

            var productDTO = _mapper.Map<ProductDTO>(product);

            await httpClient.PutAsJsonAsync($"api/admin/products/update/product/{product.Id}", productDTO);

            await httpClient.PostAsJsonAsync($"api/cart/add/products-list/{productDTO.Id}", productDTO);

            var result = await httpClient.PostAsJsonAsync($"api/cart/add/product/{productDTO.Id}", productDTO);
            if (result.IsSuccessStatusCode)
            {
                _resultAddProductToCart = true; 
            }
        }
    }
} 
