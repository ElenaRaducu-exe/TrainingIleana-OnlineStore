using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using System.Net.Http.Headers;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.WebUtilities;
using OnlineStore.Services.Contracts;
using OnlineStore.DBModels;

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

        [Inject]
        private IBrandsService _brandService { get; set; }

        [Inject]
        private ICategoriesService _categoryService { get; set; }

        public List<ProductModel> ProductsList = new(); 
        public List<Brand> BrandList = new();
        public List<Category> CategoryList = new();

        public int SelectedProductId { get; set; }
        private bool _resultAddProductToCart { get; set; } = false;
        private int _pageNumber = 1; 
        private int _pageSize = 9;
        private int _totalProducts = 0;
        private int _totalPages = 0;
        private ProductFiltersModel _filtersModel = new();
        private bool _isFiltered = false;
        private bool _isProductListEmpty = false;
        private string? _sortFilter { get; set; }
        private string? _brandNameFilter { get; set; }
        private string? _categoryNameFilter { get; set; }

        protected async Task LoadProducts()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            //var productDTOs = await httpClient.GetFromJsonAsync<List<ProductDTO>>("api/admin/products");

            var productDTOs = await httpClient.GetFromJsonAsync<List<ProductDTO>>
                        ($"api/admin/products/pagination?pageNumber={_pageNumber}&pageSize={_pageSize}");

            ProductsList = _mapper.Map<List<ProductModel>>(productDTOs);

            _totalProducts = await httpClient.GetFromJsonAsync<int>("api/admin/products/count");
            _totalPages = (int)Math.Ceiling((double)_totalProducts / _pageSize);

            BrandList = await _brandService.GetBrandsListAsync();
            CategoryList = await _categoryService.GetCategoriesAsync();
        }

        protected async Task LoadFilteredProducts()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            _filtersModel.PageNumber = _pageNumber; 
            _filtersModel.PageSize = _pageSize;

            var response = await httpClient.PostAsJsonAsync("api/admin/products/filtered", _filtersModel);

            if (response.IsSuccessStatusCode)
            {
                var productDTOs = await response.Content.ReadFromJsonAsync<List<ProductDTO>>();

                ProductsList = _mapper.Map<List<ProductModel>>(productDTOs);
            }

            var responseCountProducts = await httpClient.PostAsJsonAsync("api/admin/products/filtered/count", _filtersModel);

            if (responseCountProducts.IsSuccessStatusCode)
            {
                _totalProducts = await responseCountProducts.Content.ReadFromJsonAsync<int>();
                _totalPages = (int)Math.Ceiling((double)_totalProducts / _pageSize);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts(); 

            _resultAddProductToCart = false; 
        }

        private async Task OnPageChanged(int page)
        {
            _pageNumber = page;

            if (_isFiltered)
            {
                await LoadFilteredProducts();
            }
            else
            {
                await LoadProducts();
            }
        }

        private async Task OnSortSelected(string sortValue)
        {
            _sortFilter = sortValue; 

            var sortValues = sortValue.Split(':');

            _filtersModel.SortBy = sortValues[0];
            _filtersModel.SortDirection = sortValues[1];
        }

        protected async Task DeleteProduct(int? productId)
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

            var result = await httpClient.PostAsJsonAsync($"api/cart/add/product/{productDTO.Id}", productDTO);

            if (result.IsSuccessStatusCode)
            {
                _resultAddProductToCart = true;
            }
        }

        public async Task ApplyFilters()
        {
            _isFiltered = true;
            _pageNumber = 1;

            await LoadFilteredProducts();

            if(_totalProducts == 0)
            {
                _isProductListEmpty = true;
            }
            else
            {
                _isProductListEmpty = false;
            }

            if(_filtersModel.BrandId != null)
            {
                _brandNameFilter = BrandList.FirstOrDefault(brand => brand.Id == _filtersModel.BrandId).BrandName;
            }
            else
            {
                _brandNameFilter = null;
            }

            if (_filtersModel.CategoryId != null)
            {
                _categoryNameFilter = CategoryList.FirstOrDefault(category => category.Id == _filtersModel.CategoryId).CategoryName;
            }
            else
            {
                _categoryNameFilter = null;
            }
        }

        public async Task ResetFilters()
        {
            _isFiltered = false;
            _filtersModel = new ProductFiltersModel();
            _pageNumber = 1;
            _sortFilter = null;

            _brandNameFilter = null;
            _categoryNameFilter = null;

            await LoadProducts();
        }
    }
} 
