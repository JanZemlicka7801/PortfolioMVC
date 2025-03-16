using System.ComponentModel.DataAnnotations;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Models.DTOs;

public class ManagerDto
{
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