using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models.Configuration;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Contracts;
using System.Data;
using static MudBlazor.Icons.Custom;

namespace OnlineStore.Services
{
    public class ProductService : IProductService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DbConfiguration _dbConfiguration; 

        public ProductService(OnlineStoreContext onlineStoreContext, 
                                IMapper mapper,
                                DbConfiguration dbConfiguration)
        {
            _dbContext = onlineStoreContext;
            _mapper = mapper;
            _dbConfiguration = dbConfiguration;
        }

        public async Task<List<ProductDTO>> GetProductDTOListAsync()
        {
            try
            {
                var resultListProducts = new List<ProductDTO>();

                DataSet dataSet = new DataSet();

                using(SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using(SqlCommand sqlCommand = new SqlCommand("dbo.spGetProducts", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                        {
                            adapter.Fill(dataSet);
                        }

                        foreach(DataRow row in dataSet.Tables[0].Rows)
                        {
                            var produtDto = new ProductDTO()
                            {
                                Id = (int)row["Id"],
                                Name = row["Name"].ToString() ?? "-",
                                Description = row["Description"].ToString() ?? "-",
                                Price = (decimal)row["Price"], 
                                Stock = (int)row["Stock"],
                                ImageUrl = row["ImageUrl"].ToString() ?? "-",
                                IsActive = (bool)row["IsActive"],
                                CategoryId = (int)row["CategoryId"],
                                BrandId = (int)row["BrandId"],
                                CreatedBy = (int)row["CreatedBy"],
                                ModifiedBy = (int)row["ModifiedBy"],
                                ReservedStock = (int)row["ReservedStock"]
                            };

                            resultListProducts.Add(produtDto);
                        }

                        return resultListProducts;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in dbo.spGetProducts: {ex.Message}");
                return new List<ProductDTO>();
            }
        }

        public async Task<ProductListSummaryDTO?> GetProductList(ProductFiltersModel productFilters)
        {
            try
            {
                var resultStoredProcedure = new ProductListSummaryDTO();

                DataSet dataSet = new DataSet();

                using (SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand("spGetFilteredSortedProducts", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCommand.Parameters.AddWithValue("@PageNumber", productFilters.PageNumber); 
                        sqlCommand.Parameters.AddWithValue("@PageSize", productFilters.PageSize);

                        if (productFilters.Name != null) {
                            sqlCommand.Parameters.AddWithValue("@Name", productFilters.Name);
                        }

                        if (productFilters.BrandId != null) {
                            sqlCommand.Parameters.AddWithValue("@BrandId", productFilters.BrandId);
                        }

                        if (productFilters.CategoryId != null) {
                            sqlCommand.Parameters.AddWithValue("@CategoryId", productFilters.CategoryId);
                        }

                        if (productFilters.PriceFrom != null) {
                            sqlCommand.Parameters.AddWithValue("@PriceFrom", productFilters.PriceFrom);
                        }

                        if (productFilters.PriceTo != null) {
                            sqlCommand.Parameters.AddWithValue("@PriceTo", productFilters.PriceTo);
                        }

                        if (productFilters.AvailableStock != null) {
                            sqlCommand.Parameters.AddWithValue("@AvailableStock", productFilters.AvailableStock);
                        }

                        if (productFilters.ActiveProduct != null) {
                            sqlCommand.Parameters.AddWithValue("@ActiveProduct", productFilters.ActiveProduct);
                        }

                        if (productFilters.SortBy != null) {
                            sqlCommand.Parameters.AddWithValue("@SortBy", productFilters.SortBy);
                        }

                        if (productFilters.SortDirection != null) {
                            sqlCommand.Parameters.AddWithValue("@SortDirection", productFilters.SortDirection);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                        {
                            adapter.Fill(dataSet);
                        }

                        foreach(DataRow row in dataSet.Tables[0].Rows)
                        {
                            resultStoredProcedure.TotalCount = (int)row["TotalCount"];
                        }

                        foreach(DataRow row in dataSet.Tables[1].Rows)
                        {
                            var productDto = new ProductDTO()
                            {
                                Id = (int)row["Id"],
                                Name = row["Name"].ToString() ?? "-",
                                Description = row["Description"].ToString() ?? "-",
                                Price = (decimal)row["Price"],
                                Stock = (int)row["Stock"],
                                ImageUrl = row["ImageUrl"].ToString() ?? "-",
                                IsActive = (bool)row["IsActive"],
                                CategoryId = (int)row["CategoryId"],
                                BrandId = (int)row["BrandId"],
                                CreatedBy = (int)row["CreatedBy"],
                                ModifiedBy = (int)row["ModifiedBy"],
                                ReservedStock = (int)row["ReservedStock"]
                            };

                            resultStoredProcedure.productDTOs.Add(productDto);
                        }

                        return resultStoredProcedure; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in spGetOrderSummaryRaport: {ex.Message}");
                return null; 
            }
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

        public async Task<bool?> DeleteProduct(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

            if(product == null)
            {
                return null;
            }

            var productInCart = _dbContext.CartItems.FirstOrDefault(item => item.ProductId == id);
            var productInOrder = _dbContext.OrderItems.FirstOrDefault(item => item.ProductId == id);

            if(productInCart != null || productInOrder != null)
            {
                product.IsActive = false;

                return false;
            }

            _dbContext.Products.Remove(product);

            _dbContext.SaveChanges();

            return true;
        }

        public async Task<ProductDTO?> ChangeActiveMode(int id, int userId)
        { 
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

            if(product == null)
            {
                return null; 
            }

            product.IsActive = !product.IsActive;
            product.ModifiedBy = userId;

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
            productToUpdate.ModifiedBy = productDetails.ModifiedBy;
            productToUpdate.ReservedStock = productDetails.ReservedStock;
            productToUpdate.ModifiedAt = DateTime.UtcNow;

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

        public async Task<int?> GetAvailableStock(int productId)
        { 
            /*
            var productIdParameter = new SqlParameter()
            {
                ParameterName = "@ProductId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            var availableStockParameter = new SqlParameter()
            {
                ParameterName = "@AvailableStock",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            }; 

            await _dbContext.Database.ExecuteSqlRawAsync($"exec [dbo].[GetAvailableStock] @ProductId, @AvailableStock output", 
                        productIdParameter, availableStockParameter);

            return (int)availableStockParameter.Value; 
            */

            try
            {
                DataSet dataSet = new DataSet();

                int availableStock = 0;

                using (SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand("dbo.GetAvailableStock", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCommand.Parameters.AddWithValue("@ProductId", productId);

                        sqlCommand.Parameters.Add("@AvailableStock", SqlDbType.Int).Direction = ParameterDirection.Output;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                        {
                            adapter.Fill(dataSet);
                        }

                        availableStock = (int)sqlCommand.Parameters["@AvailableStock"].Value;
                    }
                }

                return availableStock;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in dbo.GetAvailableStock: {ex.Message}");
                return null;
            }
        }

    }
}
