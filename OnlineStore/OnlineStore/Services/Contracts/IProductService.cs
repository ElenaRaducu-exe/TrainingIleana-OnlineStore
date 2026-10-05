using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Services.Contracts
{
    public interface IProductService
    {
        Task<bool> AddProductAsync(ProductDTO productDTO);

        Task<ProductDTO?> GetProductDTOAsyncById(int id);

        Task<List<ProductDTO>> GetProductDTOListAsync(); 

        Task<List<ProductDTO>?> GetFilteredSortedProductDTOs(ProductFiltersModel productFilters); 

        Task<bool> DeleteProduct(int id);

        Task<ProductDTO> ChangeActiveMode(int id, int userId);

        Task<bool> UpdateProduct(ProductDTO productDetails);

        Task<ProductDTO?> GetProductDTOById(int productId);

        Task<List<ProductDTO>?> GetProductDTOListPagination(int pageNumber, int pageSize);

        Task<int?> GetProductsCount();

        Task<int> GetFilteredSortedProductsCount(ProductFiltersModel productFilters);

        Task<int> GetAvailableStock(int productId);
    }
}
