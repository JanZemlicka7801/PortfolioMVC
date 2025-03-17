using System.ComponentModel.DataAnnotations;
using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Controllers.Views;

public class SettingsViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public Department Department { get; set; }
}