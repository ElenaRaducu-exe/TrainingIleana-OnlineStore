using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class BrandsService : IBrandsService
    {
        private readonly OnlineStoreContext _dbContext;

        public BrandsService(OnlineStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Brand>> GetBrandsListAsync()
        {
            return _dbContext.Brands.ToList(); 
        }

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            return _dbContext.Brands.FirstOrDefault(b => b.Id == id);
        }

        public async Task<string?> GetBrandNameByIdAsync(int id)
        {
            return _dbContext.Brands.FirstOrDefault(b => b.Id == id).BrandName; 
        }

        public async Task<int?> GetIdByBrandNameAsync(string BrandName)
        {
            Brand brand = _dbContext.Brands.FirstOrDefault(b => b.BrandName == BrandName);

            if(brand == null)
            {
                return null; 
            }

            return brand.Id;
        }
    }
}
