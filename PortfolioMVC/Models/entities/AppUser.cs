using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Models.entities;

public class AppUser : IdentityUser
{
    [Required(ErrorMessage = "Username is required.")]
    public override string UserName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public override string Email { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    public Department Department { get; set; }
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}