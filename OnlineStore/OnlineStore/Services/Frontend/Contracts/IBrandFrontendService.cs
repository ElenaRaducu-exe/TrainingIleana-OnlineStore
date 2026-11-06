using OnlineStore.DBModels;

namespace OnlineStore.Services.Frontend.Contracts
{
    public interface IBrandFrontendService
    {
        Task<List<Brand>> GetBrandsListAsync(); 
        Task<string> GetBrandNameByIdAsync(int brandId);
        Task<Brand> GetBrandByIdAsync(int brandId);
    }
}
