using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class ProductService : IProductService
    {
        private OnlineStoreContext _dbConext;
        private IBrandsService _brandsService;
        private ICategoriesService _categoriesService;

        public ProductService(OnlineStoreContext onlineStoreContext, 
                                IBrandsService brandsService, ICategoriesService categoriesService)
        {
            _dbConext = onlineStoreContext;
            _brandsService = brandsService;
            _categoriesService = categoriesService;
        }

        public async Task<bool> AddProductAsync(ProductDTO productDTO)
        {
            if(productDTO.Stock < 0 || productDTO.Price <= 0 || productDTO.Name == null 
                || productDTO.Category == null || productDTO.Brand == null)
            {
                return false; 
            }

            Product newProduct = new Product()
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Stock = productDTO.Stock,
                ImageUrl = productDTO.ImageUrl,
                IsActive = productDTO.IsActive,
                CategoryId = _categoriesService.GetIdByCategoryNameAsync(productDTO.Category).Id, 
                BrandId = _brandsService.GetIdByBrandNameAsync(productDTO.Brand).Id
            };

            _dbConext.Products.Add(newProduct); 

            _dbConext.SaveChanges();

            return true; 
        }
    }
}
