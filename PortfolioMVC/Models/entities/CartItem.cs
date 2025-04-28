using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioMVC.Models.entities;

public class CartItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string CartId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}