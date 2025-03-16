using System.ComponentModel.DataAnnotations;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Models.DTOs;

public class TeamMemberDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    
    [Required]
    public Role Role { get; set; }
    
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    
    public int ProjectId { get; set; }
}