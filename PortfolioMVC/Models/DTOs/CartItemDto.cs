using System.ComponentModel.DataAnnotations;

namespace PortfolioMVC.Models.DTOs;

public class CartItemDto
{
    public int Id { get; set; }

    public string CartId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string ProductImageUrl { get; set; }

    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalPrice => Quantity * Price;
}