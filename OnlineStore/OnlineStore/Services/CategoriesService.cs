using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly OnlineStoreContext _dbContext;

        public CategoriesService(OnlineStoreContext onlineStoreContext)
        {
            _dbContext = onlineStoreContext;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return _dbContext.Categories.FirstOrDefault(c => c.Id == id); 
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return _dbContext.Categories.ToList();
        }

        public async Task<string?> GetCategoryNameByIdAsync(int id)
        {
            return _dbContext.Categories.FirstOrDefault(c => c.Id == id).CategoryName;
        }

        public async Task<int?> GetIdByCategoryNameAsync(string categoryName)
        {
            /*
             * Category category = _dbContext.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
            if(category == null)
            {
                return null; 
            }
            return category.Id;
             */
            return _dbContext.Categories.FirstOrDefault(c => c.CategoryName == categoryName).Id;
        }

        public async Task<Category?> GetBrandByName(string categoryName)
        {
            return _dbContext.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
        }
    }
}
