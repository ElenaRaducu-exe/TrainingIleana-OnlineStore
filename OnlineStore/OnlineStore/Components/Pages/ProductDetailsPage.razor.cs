using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class ProductDetailsPage : ComponentBase
    {
        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IBrandFrontendService _brandFrontendService { get; set; }

        [Inject]
        private ICategoryFrontendService _categoryFrontendService { get; set; }

        [Inject]
        private ICartFrontendService _cartFrontendService { get; set; }

        [Inject]
        private IProductFrontendService _productFrontendService { get; set; }

        [Parameter]
        public int? Id { get; set; }

        private string _brand { get; set; }
        private string _category { get; set; }

        protected ProductModel productModel { get; set; } = new();
        private bool _resultAddProductToCart { get; set; } = false;
        private bool _resultAddProductToCartUnsuccefully { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            productModel = await _productFrontendService.GetProductModelById(Id.Value); 

            if (productModel != null)
            {
                _brand = await _brandFrontendService.GetBrandNameByIdAsync(productModel.BrandId);

                _category = await _categoryFrontendService.GetCategoryNameByIdAsync(productModel.CategoryId);
            }

            _resultAddProductToCart = false;
            _resultAddProductToCartUnsuccefully = false;
        }

        protected async Task DeleteProduct(ProductModel? productToDelete)
        {
            if (productToDelete != null)
            {
                var resultDeletingProduct = await _productFrontendService.DeteleProduct(productToDelete);

                if (resultDeletingProduct == productToDelete)
                {
                    _navigation.NavigateTo("/dashboard/products");
                }

                if (resultDeletingProduct?.IsActive == false)
                {
                    productToDelete.IsActive = false;
                }

                StateHasChanged();
            }
        }

        protected void EditProduct(int productId)
        {
            _navigation.NavigateTo($"/dashboard/products/edit/{productId}");
        }

        protected async Task AddToCart(ProductModel productModel)
        {
            var addedProduct = await _cartFrontendService.AddProductToCart(productModel);

            if (addedProduct != null)
            {
                _resultAddProductToCart = true;
            }
            else
            {
                _resultAddProductToCartUnsuccefully = true;
            }
        }
    }
}
