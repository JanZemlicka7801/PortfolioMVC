using System.ComponentModel.DataAnnotations;
using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Models.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public Department Department { get; set; }

        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; } = new List<string>();

        public List<string> AllRoles { get; set; } = new List<string>();
    }
}