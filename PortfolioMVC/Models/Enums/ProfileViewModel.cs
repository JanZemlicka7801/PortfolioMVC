using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Controllers.Views;

public class ProfileViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public Department Department { get; set; }
}