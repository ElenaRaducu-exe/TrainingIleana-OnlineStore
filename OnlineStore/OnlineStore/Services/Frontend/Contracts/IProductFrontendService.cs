using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Services.Frontend.Contracts
{
    public interface IProductFrontendService
    {
        Task<bool> AddProduct(ProductModel newProduct);
        Task<bool> UpdateProduct(ProductModel productModel);
        Task<ProductModel> GetProductModelById(int productId);
        Task<ProductModel?> DeteleProduct(ProductModel productModel);
        Task<ProductListSummaryDTO?> GetProductModelList(ProductFiltersModel productFilters);
        Task<ProductModel?> UpdateActiveStatusProduct(ProductModel productModel); 
    }
}
