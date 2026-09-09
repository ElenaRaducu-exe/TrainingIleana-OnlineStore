using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class BrandsService : IBrandsService
    {
        private OnlineStoreContext _dbConext;

        public BrandsService(OnlineStoreContext dbConext)
        {
            _dbConext = dbConext;
        }

        public async Task<List<Brand>> GetBrandsListAsync()
        {
            return _dbConext.Brands.ToList(); 
        }

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            return _dbConext.Brands.FirstOrDefault(b => b.Id == id);
        }

        public async Task<string?> GetBrandNameByIdAsync(int id)
        {
            return _dbConext.Brands.FirstOrDefault(b => b.Id == id).BrandName; 
        }

        public async Task<int?> GetIdByBrandNameAsync(string BrandName)
        {
            return _dbConext.Brands.FirstOrDefault(b => b.BrandName == BrandName).Id;
        }
    }
}
