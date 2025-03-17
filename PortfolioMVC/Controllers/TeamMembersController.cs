using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Services;

namespace PortfolioMVC.Controllers;

[Authorize]
public class TeamMembersController : Controller
{
    private readonly ITeamMemberService _teamMemberService;
    private readonly IProjectService _projectService;

    public TeamMembersController(ITeamMemberService teamMemberService, IProjectService projectService)
    {
        _teamMemberService = teamMemberService;
        _projectService = projectService;
    }

    // GET: TeamMembers
    public async Task<IActionResult> Index()
    {
        var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();
        return View(teamMembers);
    }

    // GET: TeamMembers/Details
    public async Task<IActionResult> Details(int id)
    {
        var teamMember = await _teamMemberService.GetTeamMemberByIdAsync(id);
        if (teamMember == null)
        {
            return NotFound();
        }

        return View(teamMember);
    }

    // GET: TeamMembers/Create
    public async Task<IActionResult> Create()
    {
        await PopulateProjectsDropdown();
        return View();
    }

    // POST: TeamMembers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeamMemberDto teamMemberDto)
    {
        if (ModelState.IsValid)
        {
            await _teamMemberService.CreateTeamMemberAsync(teamMemberDto);
            return RedirectToAction(nameof(Index));
        }

        await PopulateProjectsDropdown();
        return View(teamMemberDto);
    }

    // GET: TeamMembers/Edit
    public async Task<IActionResult> Edit(int id)
    {
        var teamMember = await _teamMemberService.GetTeamMemberByIdAsync(id);
        if (teamMember == null)
        {
            return NotFound();
        }

        await PopulateProjectsDropdown(teamMember.ProjectId);
        return View(teamMember);
    }

    // POST: TeamMembers/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TeamMemberDto teamMemberDto)
    {
        if (id != teamMemberDto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var success = await _teamMemberService.UpdateTeamMemberAsync(id, teamMemberDto);
            if (!success)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        await PopulateProjectsDropdown(teamMemberDto.ProjectId);
        return View(teamMemberDto);
    }

    // GET: TeamMembers/Delete
    public async Task<IActionResult> Delete(int id)
    {
        var teamMember = await _teamMemberService.GetTeamMemberByIdAsync(id);
        if (teamMember == null)
        {
            return NotFound();
        }

        return View(teamMember);
    }

    // POST: TeamMembers/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _teamMemberService.DeleteTeamMemberAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateProjectsDropdown(int selectedProjectId = 0)
    {
        var projects = await _projectService.GetAllProjectsAsync();
        ViewBag.Projects = new SelectList(projects, "Id", "Name", selectedProjectId);
    }
}