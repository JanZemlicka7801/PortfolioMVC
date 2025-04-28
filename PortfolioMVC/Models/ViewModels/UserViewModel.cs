using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Department Department { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}