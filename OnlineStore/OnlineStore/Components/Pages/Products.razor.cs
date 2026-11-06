using Microsoft.AspNetCore.Components;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using AutoMapper;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class Products : ComponentBase
    {
        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        [Inject]
        private IBrandFrontendService _brandFrontendService { get; set; }

        [Inject]
        private ICategoryFrontendService _categoryFrontendService { get; set; }

        [Inject]
        private ICartFrontendService _cartFrontendService { get; set; }

        [Inject]
        private IProductFrontendService _productFrontendService { get; set; }

        public List<ProductModel> ProductsList = new();
        public List<Brand> BrandList = new();
        public List<Category> CategoryList = new();

        public int SelectedProductId { get; set; }
        private bool _resultAddProductToCart { get; set; } = false;
        private bool _resultAddProductToCartUnsuccefully { get; set; } = false;
        private int _pageNumber = 1;
        private int _pageSize = 9;
        private int _totalProducts = 0;
        private int _totalPages = 0;
        private ProductFiltersModel _filtersModel = new();
        private bool _isProductListEmpty = false;
        private string? _sortFilter { get; set; }
        private string? _brandNameFilter { get; set; }
        private string? _categoryNameFilter { get; set; }

        protected async Task LoadProducts()
        {
            _filtersModel.PageNumber = _pageNumber;
            _filtersModel.PageSize = _pageSize;

            var resultProductList = await _productFrontendService.GetProductModelList(_filtersModel);

            if (resultProductList != null)
            {
                ProductsList = _mapper.Map<List<ProductModel>>(resultProductList?.productDTOs);

                _totalProducts = resultProductList?.TotalCount ?? 0; 
                _totalPages = (int)Math.Ceiling((double)_totalProducts / _pageSize);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts();

            var responseBrands = await _brandFrontendService.GetBrandsListAsync();

            if (responseBrands != null)
            {
                BrandList = responseBrands;
            }

            var responseCategories = await _categoryFrontendService.GetCategoriesListAsync();

            if (responseCategories != null)
            {
                CategoryList = responseCategories;
            }

            _resultAddProductToCart = false;
        }

        private async Task OnPageChanged(int page)
        {
            _pageNumber = page;

            await LoadProducts();
        }

        private async Task OnSortSelected(string sortValue)
        {
            _sortFilter = sortValue;

            var sortValues = sortValue.Split(':');

            _filtersModel.SortBy = sortValues[0];
            _filtersModel.SortDirection = sortValues[1];
        }

        protected async Task DeleteProduct(ProductModel? productToDelete)
        {
            if (productToDelete != null)
            {
                var resultDeletingProduct = await _productFrontendService.DeteleProduct(productToDelete);

                if(resultDeletingProduct == productToDelete)
                {
                    ProductsList.Remove(productToDelete);
                }

                if(resultDeletingProduct?.IsActive == false)
                {
                    productToDelete.IsActive = false;   
                }

                StateHasChanged();
            }
        }

        protected async Task ChangeActiveMode(ProductModel product)
        {
            var response = await _productFrontendService.UpdateActiveStatusProduct(product);

            if (response != null)
            {
                var productList = ProductsList.FirstOrDefault(p => p.Id == response.Id);

                if (productList != null)
                {
                    productList.IsActive = response.IsActive;
                }

                StateHasChanged();
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

        public async Task ApplyFilters()
        {
            _pageNumber = 1;

            await LoadProducts();

            _isProductListEmpty = ProductsList.Count == 0 ? true : false;

            if (_filtersModel.BrandId != null)
            {
                _brandNameFilter = BrandList?.FirstOrDefault(brand => brand.Id == _filtersModel.BrandId)?.BrandName;
            }
            else
            {
                _brandNameFilter = null;
            }

            if (_filtersModel.CategoryId != null)
            {
                _categoryNameFilter = CategoryList?.FirstOrDefault(category => category.Id == _filtersModel.CategoryId)?.CategoryName;
            }
            else
            {
                _categoryNameFilter = null;
            }
        }

        public async Task ResetFilters()
        {
            _filtersModel = new ProductFiltersModel();
            _pageNumber = 1;
            _sortFilter = null;

            _brandNameFilter = null;
            _categoryNameFilter = null;

            await LoadProducts();
        }
    }
} 
