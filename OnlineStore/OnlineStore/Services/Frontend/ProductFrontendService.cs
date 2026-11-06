using AutoMapper;
using MudBlazor;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Services.Frontend
{
    public class ProductFrontendService : IProductFrontendService
    {
        private readonly IApiClient _apiClient;
        private readonly IMapper _mapper;

        private readonly IDialogService _dialogService; 

        public ProductFrontendService(IApiClient apiClient, IMapper mapper, IDialogService dialogService)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _dialogService = dialogService;
        }

        public async Task<bool> AddProduct(ProductModel newProduct)
        {
            var productDTO = _mapper.Map<ProductDTO>(newProduct);

            var response = await _apiClient.PostAsJsonAsync("api/admin/products/add/product", productDTO);

            if (response.IsSuccessStatusCode)
            {
                return true; 
            }
            
            return false;
        }

        public async Task<bool> UpdateProduct(ProductModel productModel)
        {
            var productDTO = _mapper.Map<ProductDTO>(productModel);

            var responseUpdatingProduct = await _apiClient.PutAsJsonAsync($"api/admin/products/update/product/{productDTO.Id}", productDTO);

            if (responseUpdatingProduct.IsSuccessStatusCode)
            {
                return true;
            }

            return false;   
        }

        public async Task<ProductModel> GetProductModelById(int productId)
        {
            var productDto = await _apiClient.GetAsync<ProductDTO>($"api/admin/products/get/product/{productId}"); 

            if(productDto != null)
            {
                var productModel = _mapper.Map<ProductModel>(productDto);
                return productModel; 
            }

            return new ProductModel(); 
        }

        public async Task<ProductModel?> DeteleProduct(ProductModel productModel)
        {
            var resultDeletingProduct = await _apiClient.DeleteAsync($"api/admin/products/delete/product/{productModel.Id}");

            if (resultDeletingProduct.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ProductModel();
            }

            if (resultDeletingProduct.IsSuccessStatusCode)
            {
                return productModel; 
            }

            if(resultDeletingProduct.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                productModel.IsActive = false;
                return productModel;
            }

            return null; 
        }        

        public async Task<ProductListSummaryDTO?> GetProductModelList(ProductFiltersModel productFilters)
        {
            try
            {
                var productList = await _apiClient.PostAsJsonAsync<ProductFiltersModel, ProductListSummaryDTO>("api/admin/products/list-after-rules", productFilters);
                return productList;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync("Error", $"An error occurred while fetching the product list: {ex.Message}");
             
                return null;
            }
        }

        public async Task<ProductModel?> UpdateActiveStatusProduct(ProductModel productModel)
        {
            var product = _mapper.Map<ProductDTO>(productModel);

            var resultUpdatingProduct = await _apiClient.PutAsJsonAsync($"api/admin/products/update/product/active-status/{product.Id}", product);

            if (resultUpdatingProduct.IsSuccessStatusCode)
            {
                var updatedProduct = await resultUpdatingProduct.Content.ReadFromJsonAsync<ProductDTO>();

                if (updatedProduct != null)
                {
                    return _mapper.Map<ProductModel>(updatedProduct);
                }
            }

            return null; 
        }
    }
}
