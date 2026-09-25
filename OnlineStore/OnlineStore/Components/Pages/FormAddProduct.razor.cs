using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Services.Contracts;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.Pages
{
    public partial class FormAddProduct : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IProductService _productService { get; set; }

        [Inject]
        private IBrandsService _brandService { get; set; }

        [Inject]
        private ICategoriesService _categoryService { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        private ProductModel _productModel = new();

        private List<Brand> _brands = new();
        private List<Category> _categories = new();
        private string _message = string.Empty;

        private bool _priceError = false;
        private string _priceErrorMessage = string.Empty;
        private bool _stockError = false;
        private string _stockErrorMessage = string.Empty;

        [Parameter]
        public int? productId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            _brands = await httpClient.GetFromJsonAsync<List<Brand>>("api/brands");
            _categories = await httpClient.GetFromJsonAsync<List<Category>>("api/categories");

            if(productId != null)
            {
                var _productDTO = await _productService.GetProductDTOAsyncById(productId.Value);

                if(_productDTO != null)
                {
                    _productModel = _mapper.Map<ProductModel>(_productDTO);
                }
            }
        }

        private async Task AddProduct()
        {
            if (_productModel.Stock < 0)
            {
                _stockError = true;
                _stockErrorMessage = "Stock has to be at least 0!";
            }

            if (_productModel.Price <= 0)
            {
                _priceError = true;
                _priceErrorMessage = "Price can not be 0 or negative!";
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            var productDTO = _mapper.Map<ProductModel>(_productModel);
            var response = await httpClient.PostAsJsonAsync("api/admin/products/add/product", productDTO);

            if (response.IsSuccessStatusCode)
            {
                _productModel = new ProductModel();
                _message = "Product added successfully!";

                _stockError = false;
                _stockErrorMessage = string.Empty;
                _priceError = false;
                _priceErrorMessage = string.Empty;

                await Task.Delay(2000);
                _navigation.NavigateTo("/dashboard/products");
            }
            else
            {
                _message = "Product added unsuccessfully!";
            }
        }

        protected async Task EditProduct(ProductModel productModel)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            var productDTO = _mapper.Map<ProductDTO>(_productModel);
            var response = await httpClient.PutAsJsonAsync($"api/admin/products/update/product/{productDTO.Id}", productDTO);

            if (response.IsSuccessStatusCode)
            {
                _navigation.NavigateTo("/dashboard/products");
            }
        }
    }
}
