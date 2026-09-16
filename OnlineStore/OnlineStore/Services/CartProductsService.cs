using OnlineStore.Components.Pages;
using OnlineStore.Models;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class CartProductsService : ICartProductsService
    {
        private List<ProductDTO> _cartProducts = new List<ProductDTO>();

        public async Task AddProductToCartProductsList(ProductDTO productDTO)
        { 
            if(productDTO != null)
            {
                _cartProducts.Add(productDTO);
            }
        }

        public async Task<List<ProductDTO>?> GetCartProducts() 
        { 
            return _cartProducts; 
        }
    }
}
