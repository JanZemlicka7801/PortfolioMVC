using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.entities;
using PortfolioMVC.Services;
using System.Security.Claims;

namespace PortfolioMVC.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ITeamMemberService _teamMemberService;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;

    public DashboardController(
        IProjectService projectService,
        ITeamMemberService teamMemberService,
        UserManager<AppUser> userManager,
        AppDbContext context)
    {
        _projectService = projectService;
        _teamMemberService = teamMemberService;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewBag.ProjectCount = await _context.Projects.CountAsync();
        ViewBag.TeamMemberCount = await _context.TeamMembers.CountAsync();
        ViewBag.UserProjects = await _context.Projects
            .Where(p => p.ManagerId == userId)
            .ToListAsync();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            ViewBag.UserName = currentUser.Name;
            ViewBag.Department = currentUser.Department;
        }

        var recentProjects = await _context.Projects
            .OrderByDescending(p => p.StartDate)
            .Take(5)
            .ToListAsync();

        return View(recentProjects);
    }
}