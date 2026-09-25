using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class ProductService : IProductService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IBrandsService _brandsService;
        private readonly ICategoriesService _categoriesService;
        private readonly IMapper _mapper;

        public ProductService(OnlineStoreContext onlineStoreContext, 
                                IBrandsService brandsService, 
                                ICategoriesService categoriesService,
                                IMapper mapper)
        {
            _dbContext = onlineStoreContext;
            _brandsService = brandsService;
            _categoriesService = categoriesService;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>?> GetProductDTOListAsync()
        {
            return await _dbContext.Database.SqlQuery<ProductDTO>($"exec dbo.spGetProducts").ToListAsync();

            //List<Product> productDTOs = _dbContext.Products.ToList();
            //return _mapper.Map<List<ProductDTO>>(productDTOs);
        }

        public async Task<List<ProductDTO>?> GetProductDTOListPagination(int pageNumber, int pageSize)
        {
            return await _dbContext.Database.SqlQuery<ProductDTO>($"spGetProductsPagination @PageNumber={pageNumber}, @PageSize={pageSize}").ToListAsync();
        }

        public async Task<bool> AddProductAsync(ProductDTO productDTO)
        {
            if(productDTO.Stock < 0 || productDTO.Price <= 0 || productDTO.Name == null 
                || productDTO.BrandId == null || productDTO.CategoryId == null)
            {
                return false; 
            }

            Product newProduct = _mapper.Map<Product>(productDTO);

            _dbContext.Products.Add(newProduct); 

            await _dbContext.SaveChangesAsync();

            return true; 
        }

        public async Task<ProductDTO?> GetProductDTOAsyncById(int id)
        {
            var product = _dbContext.Products.AsNoTracking().FirstOrDefault(p =>  p.Id == id);

            ProductDTO productDTO = _mapper.Map<ProductDTO>(product);

            if(productDTO == null)
            {
                return null; 
            }

            return productDTO;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

            if(product == null)
            {
                return false;
            }
            else
            {
                _dbContext.Products.Remove(product);

                _dbContext.SaveChanges();

                return true;
            }
        }

        public async Task<ProductDTO?> ChangeActiveMode(int id)
        { 
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

            if(product == null)
            {
                return null; 
            }

            product.IsActive = !product.IsActive;

            _dbContext.SaveChanges();

            return await GetProductDTOAsyncById(product.Id);
        }

        public async Task<bool> UpdateProduct(ProductDTO productDetails)
        {
            if (productDetails == null)
            {
                return false; 
            }

            var productToUpdate = _dbContext.Products.FirstOrDefault(p => p.Id == productDetails.Id);

            if (productToUpdate == null)
            {
                return false;
            }

            productToUpdate.Name = productDetails.Name;
            productToUpdate.Description = productDetails.Description;
            productToUpdate.Price = productDetails.Price;
            productToUpdate.Stock = productDetails.Stock;
            productToUpdate.ImageUrl = productDetails.ImageUrl;
            productToUpdate.IsActive = productDetails.IsActive;
            productToUpdate.BrandId = productDetails.BrandId;
            productToUpdate.CategoryId = productDetails.CategoryId;

            await _dbContext.SaveChangesAsync();

            // -------
            //return await GetProductDTOAsync(productToUpdate.Id); 
            return true; 
        }

        public async Task<ProductDTO?> GetProductDTOById(int productId)
        {
            var product = _dbContext.Products.FirstOrDefault(product => product.Id == productId);

            if(product == null)
            {
                return null;
            }

            var productDTO = _mapper.Map<ProductDTO>(product);
            return productDTO;
        }

        public async Task<int?> GetProductsCount()
        {
            return await _dbContext.Products.CountAsync();
        }
    }
}
