using System.ComponentModel.DataAnnotations;

namespace PortfolioMVC.Models.entities;

public class Manager
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    public Department Department { get; set; }
    
    public string? Picture { get; set; }
}

public enum Department
{
    It,
    UxUi,
    Marketing,
    Business,
    Hr,
    Accounting
}