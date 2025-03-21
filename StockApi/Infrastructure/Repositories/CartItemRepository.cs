﻿using MongoDB.Driver;
using StockApi.Domain.Entities;
using StockApi.Domain.Interfaces;

namespace StockApi.Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly IMongoCollection<CartItem> _cartItems;
        public CartItemRepository(IMongoDatabase database)
        {
            _cartItems = database.GetCollection<CartItem>("CartItems");
        }


        public async Task<bool> DeleteCartItem(string cartItemId)
        {
            if (string.IsNullOrEmpty(cartItemId))
            {
                throw new ArgumentException("Please provide an ID");
            }
            var result = await _cartItems.DeleteOneAsync(c => c.Id == cartItemId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }


        public async Task<List<CartItem>> GetAllCartItems()
        {
            var cartItems = await _cartItems.Aggregate()
                .Lookup("users", "UserId", "Id", "User")
                .Lookup("products", "ProductId", "Id", "Product")
                .As<CartItem>()
                .ToListAsync();

            return cartItems;
        }

        public async Task<CartItem> GetCartItemById(string cartItemId)
        {
            var cartItem = await _cartItems.Aggregate()
                .Lookup("users", "UserId", "Id", "User")
                .Lookup("products", "ProductId", "Id", "Product")
                .As<CartItem>()
                .FirstOrDefaultAsync();
            if (cartItem==null)
            {
                throw new Exception("CartItem does not exist");
            }
            return cartItem;
        }

        public async Task<bool> UpdateQuantity(string cartItemId, int newQuantity)
        {
            var cartItem = await _cartItems.Find(c => c.Id == cartItemId).FirstOrDefaultAsync();
            if (cartItem == null)
            {
                throw new Exception("CartItem does not exist");
            }
            if (cartItem.Quantity == 0)
            {
                cartItem.Quantity = newQuantity;
            }
            var result = await _cartItems.UpdateOneAsync(
                           c => c.Id == cartItemId,
                           Builders<CartItem>.Update.Set
                           (c => c.Quantity, cartItem.Quantity));
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
