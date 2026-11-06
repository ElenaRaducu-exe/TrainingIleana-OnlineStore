using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Services.Frontend
{
    public class BrandFrontendService : IBrandFrontendService
    {
        private readonly IApiClient _apiClient;

        public BrandFrontendService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Brand>> GetBrandsListAsync()
        {
            var brandList = await _apiClient.GetAsync<List<Brand>>("api/brands");

            if (!brandList.Any())
            {
                return new List<Brand>();
            }

            return brandList;
        }

        public async Task<string> GetBrandNameByIdAsync(int brandId)
        {
            var brandName = await _apiClient.GetAsync<string>($"api/brands/get/name/{brandId}");

            if(string.IsNullOrEmpty(brandName))
            {
                throw new Exception($"Brand with ID {brandId} not found.");
            }

            return brandName;
        }

        public async Task<Brand> GetBrandByIdAsync(int brandId)
        {
            var brand = await _apiClient.GetAsync<Brand?>($"api/brands/get/{brandId}");

            if (brand == null)
            {
                throw new Exception($"Brand with ID {brandId} not found.");
            }

            return brand;
        }
    }
}