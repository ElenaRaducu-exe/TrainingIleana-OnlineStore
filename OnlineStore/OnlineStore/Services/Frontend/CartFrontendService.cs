using AutoMapper;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Services.Frontend
{
    public class CartFrontendService : ICartFrontendService 
    {
        private readonly IApiClient _apiClient;
        private readonly IMapper _mapper;

        public CartFrontendService(IApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        public async Task<ProductModel?> AddProductToCart(ProductModel productModel)
        {
            var productDTO = _mapper.Map<ProductDTO>(productModel); 

            var resultAddingProductToCart = await _apiClient.PostAsJsonAsync<ProductDTO>($"api/cart/add/product/{productDTO.Id}", productDTO);

            if (resultAddingProductToCart.IsSuccessStatusCode)
            {
                productModel = _mapper.Map<ProductModel>(productDTO);

                productModel.ReservedStock++;
                productModel.IsActive = productModel.ReservedStock < productModel.Stock;

                return productModel;
            }

            return null;
        }

        public async Task<bool> DeleteCartItem(int cartItemId)
        {
            var resultDeletingCartItem = await _apiClient.DeleteAsync($"api/cart/delete/{cartItemId}");

            if(resultDeletingCartItem.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteCart(int cartId)
        {
            var resultDeletingCart = await _apiClient.DeleteAsync($"api/cart/delete/cart/{cartId}");

            if (resultDeletingCart.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<CartSummaryPageModel?> GetCartItemsByUser()
        {
            var result = await _apiClient.GetAsync<CartSummaryPageModelDto>("api/cart/items/user");

            if(result != null)
            {
                var cartItems = _mapper.Map<CartSummaryPageModel>(result);
                return cartItems;
            }

            return new CartSummaryPageModel();
        }

        public async Task<bool> UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            var resultUpdatingCartItemQuantity = await _apiClient.PutAsJsonAsync($"api/cart/update/quantity/{cartItemId}", newQuantity);

            if (resultUpdatingCartItemQuantity.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
