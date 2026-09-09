using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class CategoriesService : ICategoriesService
    {
        private OnlineStoreContext _dbConext;

        public CategoriesService(OnlineStoreContext onlineStoreContext)
        {
            _dbConext = onlineStoreContext;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return _dbConext.Categories.FirstOrDefault(c => c.Id == id); 
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return _dbConext.Categories.ToList();
        }

        public async Task<string?> GetCategoryNameByIdAsync(int id)
        {
            return _dbConext.Categories.FirstOrDefault(c => c.Id == id).CategoryName;
        }

        public async Task<int?> GetIdByCategoryNameAsync(string categoryName)
        {
            /*
             * Category category = _dbConext.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
            if(category == null)
            {
                return null; 
            }
            return category.Id;
             */
            return _dbConext.Categories.FirstOrDefault(c => c.CategoryName == categoryName).Id;
        }
    }
}
