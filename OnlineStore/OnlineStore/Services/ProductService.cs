using Microsoft.AspNetCore.Components;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class ProductService : IProductService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IBrandsService _brandsService;
        private readonly ICategoriesService _categoriesService;

        public ProductService(OnlineStoreContext onlineStoreContext, 
                                IBrandsService brandsService, ICategoriesService categoriesService)
        {
            _dbContext = onlineStoreContext;
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
                CategoryId = (int)_categoriesService.GetIdByCategoryNameAsync(productDTO.Category).Result, 
                BrandId = (int)_brandsService.GetIdByBrandNameAsync(productDTO.Brand).Result
            };

            _dbContext.Products.Add(newProduct); 

            await _dbContext.SaveChangesAsync();

            return true; 
        }

        public async Task<ProductDTO?> GetProductDTOAsync(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p =>  p.Id == id);
            string brandName = _brandsService.GetBrandNameByIdAsync(product.BrandId).Result; 
            string categoryName = _categoriesService.GetCategoryNameByIdAsync(product.CategoryId).Result;

            ProductDTO productDTO = new ProductDTO()
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock, 
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                Category = categoryName, 
                Brand = brandName
            }; 

            return productDTO;
        }

        public async Task<List<ProductDTO>?> GetProductDTOListAsync()
        {
            List<Product> products = _dbContext.Products.ToList();
            List<ProductDTO> result = new List<ProductDTO>();
            
            if(products != null)
            {
                foreach (var product in products)
                {
                    ProductDTO productDTO = await GetProductDTOAsync(product.Id);

                    if (productDTO != null)
                    {
                        result.Add(productDTO);
                    }
                }
            }
            else
            {
                return null; 
            }

            return result;
        }
    }
}
