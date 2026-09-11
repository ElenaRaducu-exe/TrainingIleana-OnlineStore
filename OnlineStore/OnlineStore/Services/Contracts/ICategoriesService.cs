using OnlineStore.DBModels;

namespace OnlineStore.Services.Contracts
{
    public interface ICategoriesService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<string?> GetCategoryNameByIdAsync(int id);
        Task<int?> GetIdByCategoryNameAsync(string categoryName);
        Task<Category?> GetBrandByName(string categoryName);
    }
}
