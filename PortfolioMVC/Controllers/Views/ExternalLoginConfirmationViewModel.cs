using System.ComponentModel.DataAnnotations;
using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Controllers.Views
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public Department Department { get; set; }
    }
}