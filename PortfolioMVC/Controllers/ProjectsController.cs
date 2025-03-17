using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Services;

namespace PortfolioMVC.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }


    public async Task<IActionResult> Index()
    {
        var projects = await _projectService.GetAllProjectsAsync();
        return View(projects);
    }


    public async Task<IActionResult> Details(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }


    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectDto projectDto)
    {
        if (ModelState.IsValid)
        {
            await _projectService.CreateProjectAsync(projectDto);
            return RedirectToAction(nameof(Index));
        }
        return View(projectDto);
    }


    public async Task<IActionResult> Edit(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectDto projectDto)
    {
        if (id != projectDto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var success = await _projectService.UpdateProjectAsync(id, projectDto);
            if (!success)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        return View(projectDto);
    }


    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _projectService.DeleteProjectAsync(id);
        return RedirectToAction(nameof(Index));
    }
}