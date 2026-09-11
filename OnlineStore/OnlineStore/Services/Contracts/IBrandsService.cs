using OnlineStore.DBModels;

namespace OnlineStore.Services.Contracts
{
    public interface IBrandsService
    {
        Task<List<Brand>> GetBrandsListAsync();

        Task<Brand?> GetBrandByIdAsync(int id);

        Task<string?> GetBrandNameByIdAsync(int id);

        Task<int?> GetIdByBrandNameAsync(string BrandName);

        Task<Brand?> GetBrandByName(string brandName);
    }
}
