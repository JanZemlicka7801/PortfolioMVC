using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioMVC.Models;

public class TeamMember
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    
    [Required]
    public Role Role { get; set; }
    
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    
    // Foreign key to project
    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
}

public enum Role
{
    Developer,
    Analyst,
    ProjectSponsor,
    Architect,
    Designer,
    Tester,
    ScrumMaster
}