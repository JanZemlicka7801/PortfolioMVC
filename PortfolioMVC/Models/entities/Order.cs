using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioMVC.Models.entities;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string OrderNumber { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    [ForeignKey("AppUser")]
    public string UserId { get; set; }

    public AppUser User { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}