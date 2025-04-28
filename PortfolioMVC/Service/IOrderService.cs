using PortfolioMVC.Models.DTOs;

namespace PortfolioMVC.Service
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string userId, string cartId);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto?> GetOrderDetailsAsync(int orderId, string userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, Models.Enums.OrderStatus status);
    }
}