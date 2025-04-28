using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Models.DTOs;

public class OrderDto
{
    public int Id { get; set; }

    public string OrderNumber { get; set; }

    public DateTime OrderDate { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}