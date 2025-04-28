using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioMVC.Models.entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [StringLength(200)]
    public string ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Required]
    public ProductCategory Category { get; set; }

    public bool IsApproved { get; set; } = false;

    [ForeignKey("AppUser")]
    public string CreatedById { get; set; }

    public AppUser CreatedBy { get; set; }
}