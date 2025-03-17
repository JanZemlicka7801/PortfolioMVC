using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using System.Security.Claims;

namespace PortfolioMVC.ViewComponents
{
    public class UserProjectsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public UserProjectsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var projects = await _context.Projects
                .Where(p => p.ManagerId == userId)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ManagerId = p.ManagerId
                })
                .ToListAsync();

            return View(projects);
        }
    }
}