using System.ComponentModel.DataAnnotations;
using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Controllers.Views;

// References: https://www.youtube.com/watch?v=B0_gM-wBlmE
public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Department is required.")]
    public Department Department { get; set; }
        
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Confirm your password.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}