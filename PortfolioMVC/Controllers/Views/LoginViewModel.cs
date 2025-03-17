namespace PortfolioMVC.Controllers.Views;

using System.ComponentModel.DataAnnotations;

// References: https://www.youtube.com/watch?v=B0_gM-wBlmE
public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
