using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Components.Pages;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models.Configuration;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.StoredProcedureModels;
using OnlineStore.Services.Contracts;
using System.Data;

namespace OnlineStore.Services
{
    public class CartProductsService : ICartProductsService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IMapper _mapper;

        private readonly DbConfiguration _dbConfiguration; 

        public CartProductsService(OnlineStoreContext onlineStoreContext, 
                                   IMapper mapper,
                                   DbConfiguration dbConfiguration)
        {
            _dbContext = onlineStoreContext;
            _mapper = mapper;
            _dbConfiguration = dbConfiguration;
        }

        public async Task<List<CartItemSummaries>?> GetCartItems()
        {
            try
            {
                var resultStoredProcedure = new List<CartItemSummaries>(); 

                DataSet dataSet = new DataSet();

                using(SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using(SqlCommand sqlCommand = new SqlCommand("dbo.spGetCartItemsDetails", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                        {
                            adapter.Fill(dataSet);
                        }

                        foreach(DataRow row in dataSet.Tables[0].Rows)
                        {
                            var cartItemSummary = new CartItemSummaries
                            {
                                CartId = Convert.ToInt32(row["CartId"]),
                                UserId = Convert.ToInt32(row["UserId"]),
                                CartItemId = Convert.ToInt32(row["CartItemId"]),
                                Quantity = Convert.ToInt32(row["Quantity"]),
                                ProductId = Convert.ToInt32(row["ProductId"]),
                                Username = row["Username"].ToString() ?? "-",
                                ProductName = row["ProductName"].ToString() ?? "-",
                                Description = row["Description"].ToString() ?? "-",
                                Price = Convert.ToDecimal(row["Price"]),
                                ImageUrl = row["ImageUrl"].ToString() ?? "-",
                                CategoryName = row["CategoryName"].ToString() ?? "-",
                                BrandName = row["BrandName"].ToString() ?? "-"
                            };

                            resultStoredProcedure.Add(cartItemSummary);
                        }

                        return resultStoredProcedure;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in dbo.spGetCartItemsDetails: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CartItemDTO>?> GetCartItemsByUser(int userId)
        {
            try
            {
                var resultStoredProcedure = new List<CartItemSummaries>();

                DataSet dataSet = new DataSet();

                using (SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand("dbo.spGetCartItemsDetailsUser", connection) { CommandType = CommandType.StoredProcedure })
                    {

                        sqlCommand.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                        {
                            adapter.Fill(dataSet);
                        }

                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            var cartItemSummary = new CartItemSummaries
                            {
                                CartId = Convert.ToInt32(row["CartId"]),
                                UserId = Convert.ToInt32(row["UserId"]),
                                CartItemId = Convert.ToInt32(row["CartItemId"]),
                                Quantity = Convert.ToInt32(row["Quantity"]),
                                ProductId = Convert.ToInt32(row["ProductId"]),
                                Username = row["Username"].ToString() ?? "-",
                                ProductName = row["ProductName"].ToString() ?? "-",
                                Description = row["Description"].ToString() ?? "-",
                                Price = Convert.ToDecimal(row["Price"]),
                                ImageUrl = row["ImageUrl"].ToString() ?? "-",
                                CategoryName = row["CategoryName"].ToString() ?? "-",
                                BrandName = row["BrandName"].ToString() ?? "-"
                            };

                            resultStoredProcedure.Add(cartItemSummary);
                        }

                        return _mapper.Map<List<CartItemDTO>>(resultStoredProcedure);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in dbo.spGetCartItemsDetails: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddProductToCart(int productId, int userId)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if(product == null || product.ReservedStock >= product.Stock)
            {
                return false; 
            }

            var existingCart = _dbContext.Carts.FirstOrDefault(c => c.UserId == userId);

            if(existingCart != null)
            {
                var existingCartItem = _dbContext.CartItems.FirstOrDefault(c => c.ProductId == productId && c.CartId == existingCart.Id);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity++;
                }
                else
                {
                    var cartItem = new CartItem()
                    {
                        Quantity = 1,
                        ProductId = productId,
                        CartId = existingCart.Id
                    };

                    await _dbContext.CartItems.AddAsync(cartItem);
                }
            }
            else
            {
                var cart = new Cart()
                {
                    UserId = userId
                };

                await _dbContext.Carts.AddAsync(cart);

                var cartItem = new CartItem()
                {
                    Quantity = 1,
                    ProductId = productId,
                    Cart = cart
                };

                await _dbContext.CartItems.AddAsync(cartItem);
            }

            product.ReservedStock++;
            product.IsActive = product.ReservedStock < product.Stock;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            if(newQuantity < 1)
            {
                return false;
            }

            var cartItem = await _dbContext.CartItems.FirstOrDefaultAsync(item => item.Id == cartItemId);

            if(cartItem  == null )
            {
                return false; 
            }

            int initialQuantity = cartItem.Quantity;

            var product = await _dbContext.Products.FirstOrDefaultAsync(product => product.Id == cartItem.ProductId);

            if( product == null )
            {
                return false; 
            }

            int quantityDifference = newQuantity - initialQuantity;
            int newReservedStock = product.ReservedStock + quantityDifference;

            if(newReservedStock < 0 || newReservedStock > product.Stock)
            {
                return false; 
            }
     
            product.ReservedStock = newReservedStock;
            product.IsActive = product.ReservedStock < product.Stock;

            cartItem.Quantity = newQuantity;

            await _dbContext.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> DeleteCartItem(int cartItemId) 
        {
            var cartItem = _dbContext.CartItems.FirstOrDefault(item => item.Id == cartItemId);

            if (cartItem == null)
            {
                return false;
            }

            var cart = _dbContext.Carts.FirstOrDefault(cart => cart.Id == cartItem.CartId);

            if (cart == null)
            {
                return false;
            }

            var product = _dbContext.Products.FirstOrDefault(product => product.Id == cartItem.ProductId);
            if(product == null)
            {
                return false; 
            }

            if(product.ReservedStock < cartItem.Quantity)
            {
                return false;
            }

            product.ReservedStock -= cartItem.Quantity;
            product.IsActive = product.ReservedStock < product.Stock;

            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();

            if(!_dbContext.CartItems.Any(item => item.CartId == cart.Id))
            {
                _dbContext.Carts.Remove(cart);
                await _dbContext.SaveChangesAsync();
            }

            return true; 
        }

        public async Task<bool> DeleteCart(int cartId)
        {
            var cart = _dbContext.Carts.FirstOrDefault(cart => cart.Id == cartId);

            if(cart == null)
            {
                return false; 
            }

            var cartItems = _dbContext.CartItems.Where(item => item.CartId == cartId).ToList();

            if(cartItems == null)
            {
                return false; 
            }

            foreach(var cartItem in cartItems)
            {
                var product = _dbContext.Products.FirstOrDefault(product => product.Id == cartItem.ProductId);

                if (product == null)
                {
                    return false; 
                }

                product.ReservedStock -= cartItem.Quantity;
                product.IsActive = product.ReservedStock < product.Stock;

                _dbContext.CartItems.Remove(cartItem);
            }

            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}