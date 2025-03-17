using Microsoft.AspNetCore.Identity;
using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Models.entities;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
    public Department Department { get; set; }
    public string Picture { get; set; }
}