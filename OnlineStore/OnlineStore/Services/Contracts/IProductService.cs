using OnlineStore.Models;

namespace OnlineStore.Services.Contracts
{
    public interface IProductService
    {
        Task<bool> AddProductAsync(ProductDTO productDTO);

        Task<ProductDTO?> GetProductDTOAsync(int id);

        Task<List<ProductDTO>> GetProductDTOListAsync(); 
    }
}
