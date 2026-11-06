using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Services.Frontend.Contracts; 

namespace OnlineStore.Services.Frontend
{
    public class CategoryFrontendService : ICategoryFrontendService
    {
        private readonly IApiClient _apiClient;

        public CategoryFrontendService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Category>> GetCategoriesListAsync()
        {
            var categoryList = await _apiClient.GetAsync<List<Category>>("api/categories");

            if (!categoryList.Any())
            {
                return new List<Category>();
            }

            return categoryList;
        }

        public async Task<string> GetCategoryNameByIdAsync(int categoryId)
        {
            var categoryName = await _apiClient.GetAsync<string?>($"api/categories/get/name/{categoryId}");

            if(string.IsNullOrEmpty(categoryName))
            {
                throw new Exception($"Category with ID {categoryId} not found.");
            } 

            return categoryName;
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _apiClient.GetAsync<Category?>($"api/categories/get/{categoryId}");

            if (category == null)
            {
                throw new Exception($"Category with ID {categoryId} not found.");
            }

            return category;
        }

    }
}
