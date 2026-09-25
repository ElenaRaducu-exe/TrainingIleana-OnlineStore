using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Contracts
{
    public interface IProductService
    {
        Task<bool> AddProductAsync(ProductDTO productDTO);

        Task<ProductDTO?> GetProductDTOAsyncById(int id);

        Task<List<ProductDTO>> GetProductDTOListAsync(); 

        Task<bool> DeleteProduct(int id);

        Task<ProductDTO> ChangeActiveMode(int id);

        Task<bool> UpdateProduct(ProductDTO productDetails);

        Task<ProductDTO?> GetProductDTOById(int productId);

        Task<List<ProductDTO>?> GetProductDTOListPagination(int pageNumber, int pageSize);

        Task<int?> GetProductsCount();
    }
}
