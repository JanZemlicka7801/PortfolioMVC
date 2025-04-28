using System.ComponentModel.DataAnnotations;
using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Models.DTOs;

public class ProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [StringLength(200)]
    public string ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Required(ErrorMessage = "Category is required.")]
    public ProductCategory Category { get; set; }

    public bool IsApproved { get; set; }

    public string CreatedById { get; set; }
}