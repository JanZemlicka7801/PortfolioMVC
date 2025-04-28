using PortfolioMVC.Models.DTOs;

namespace PortfolioMVC.Service
{
    public interface ICartService
    {
        Task<string> GetCartIdAsync(string userId);
        Task<IEnumerable<CartItemDto>> GetCartItemsAsync(string cartId);
        Task<CartItemDto> AddToCartAsync(string cartId, int productId, int quantity);
        Task<bool> RemoveFromCartAsync(string cartId, int cartItemId);
        Task<bool> UpdateCartQuantityAsync(string cartId, int cartItemId, int quantity);
        Task<decimal> GetCartTotalAsync(string cartId);
        Task ClearCartAsync(string cartId);
    }
}