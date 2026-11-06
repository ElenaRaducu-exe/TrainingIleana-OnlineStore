using OnlineStore.DBModels;

namespace OnlineStore.Services.Frontend.Contracts
{
    public interface ICategoryFrontendService
    {
        Task<List<Category>> GetCategoriesListAsync();
        Task<string> GetCategoryNameByIdAsync(int categoryId);
        Task<Category> GetCategoryByIdAsync(int categoryId);
    }
}
