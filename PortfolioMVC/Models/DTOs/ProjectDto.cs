using System.ComponentModel.DataAnnotations;

namespace PortfolioMVC.Models.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
    
    public int ManagerId { get; set; }
}