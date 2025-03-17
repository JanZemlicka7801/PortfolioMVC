using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;

namespace PortfolioMVC.ViewComponents
{
    public class ProjectTeamMembersViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ProjectTeamMembersViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int projectId)
        {
            var teamMembers = await _context.TeamMembers
                .Where(tm => tm.ProjectId == projectId)
                .Select(tm => new TeamMemberDto
                {
                    Id = tm.Id,
                    Name = tm.Name,
                    Role = tm.Role,
                    Email = tm.Email,
                    ProjectId = tm.ProjectId
                })
                .ToListAsync();

            return View(teamMembers);
        }
    }
}