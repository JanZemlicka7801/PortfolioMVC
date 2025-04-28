using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Service
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public Task<string> GetCartIdAsync(string userId)
        {
            return Task.FromResult(userId);
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsAsync(string cartId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.CartId == cartId)
                .Include(c => c.Product)
                .ToListAsync();

            return cartItems.Select(MapToDto);
        }

        public async Task<CartItemDto> AddToCartAsync(string cartId, int productId, int quantity)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsApproved && p.IsAvailable);

            if (product == null)
                throw new Exception("Product not found or not available");

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return new CartItemDto
            {
                Id = cartItem.Id,
                CartId = cartItem.CartId,
                ProductId = cartItem.ProductId,
                ProductName = product.Name,
                ProductImageUrl = product.ImageUrl,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price
            };
        }

        public async Task<bool> RemoveFromCartAsync(string cartId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.Id == cartItemId);

            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateCartQuantityAsync(string cartId, int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.Id == cartItemId);

            if (cartItem == null)
                return false;

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetCartTotalAsync(string cartId)
        {
            return await _context.CartItems
                .Where(c => c.CartId == cartId)
                .SumAsync(c => c.Price * c.Quantity);
        }

        public async Task ClearCartAsync(string cartId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        private CartItemDto MapToDto(CartItem cartItem)
        {
            return new CartItemDto
            {
                Id = cartItem.Id,
                CartId = cartItem.CartId,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                ProductImageUrl = cartItem.Product.ImageUrl,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price
            };
        }
    }
}
