using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Backend.Contracts;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class FormAddProduct : ComponentBase
    {
        [Inject]
        private IApiClient _apiClient { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        [Inject]
        private IProductService _productService { get; set; }

        [Inject]
        private IBrandFrontendService _brandFrontendService { get; set; }

        [Inject]
        private ICategoryFrontendService _categoryFrontendService { get; set; }

        [Inject]
        private IProductFrontendService _productFrontendService { get; set; }

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
            _brands = await _brandFrontendService.GetBrandsListAsync();
            _categories = await _categoryFrontendService.GetCategoriesListAsync();

            if (productId != null)
            {
                _productModel = await _productFrontendService.GetProductModelById(productId.Value); 
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

            var resultProductAdded = await _productFrontendService.AddProduct(_productModel);

            if (resultProductAdded)
            {
                _productModel = new ProductModel();
                _message = "Product added successfully!";

                _stockError = false;
                _stockErrorMessage = string.Empty;
                _priceError = false;
                _priceErrorMessage = string.Empty;

                await Task.Delay(3000);
                _navigation.NavigateTo("/dashboard/products");
            }
            else
            {
                _message = "Product added unsuccessfully!";
            }
        }

        protected async Task EditProduct(ProductModel productModel)
        {
            var response = await _productFrontendService.UpdateProduct(productModel);

            if (response)
            {
                _navigation.NavigateTo("/dashboard/products");
            }
        }
    }
}
