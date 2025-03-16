using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ProjectController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a list of projects from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains an ActionResult with an IEnumerable of ProjectDto objects representing the projects.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var projects = await _context.Projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            ManagerId = p.ManagerId
        }).ToListAsync();
        
        return Ok(projects);
    }

    /// <summary>
    /// Retrieves a specific project from the database by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains an ActionResult with a ProjectDto object representing the project if found,
    /// or a NotFound result if the project does not exist.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _context.Projects
            .Where(p => p.Id == id)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ManagerId = p.ManagerId
            })
            .FirstOrDefaultAsync();

        if (project == null)
        {
            return NotFound();
        }
        
        return Ok(project);
    }

    /// <summary>
    /// Updates an existing project in the database based on the provided project details.
    /// </summary>
    /// <param name="id">The ID of the project to update.</param>
    /// <param name="updatedProject">The updated project object containing new data.</param>
    /// <returns>An IActionResult indicating the result of the operation.
    /// Returns BadRequest if the ID does not match, NotFound if the project does not exist,
    /// or NoContent if the update is successful.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProject(int id, [FromBody] Project updatedProject)
    {
        if (id != updatedProject.Id)
        {
            return BadRequest();
        }

        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        
        project.Name = updatedProject.Name;
        project.Description = updatedProject.Description;
        project.StartDate = updatedProject.StartDate;
        project.EndDate = updatedProject.EndDate;
        project.ManagerId = updatedProject.ManagerId;
        
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Creates a new project and saves it to the database.
    /// </summary>
    /// <param name="createProject">The project object containing the data for the new project.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an ActionResult with the created ProjectDto.</returns>
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject([FromBody] Project createProject)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var project = new Project
        {
            Id = createProject.Id,
            Name = createProject.Name,
            Description = createProject.Description,
            StartDate = createProject.StartDate,
            EndDate = createProject.EndDate,
            ManagerId = createProject.ManagerId
        };
        
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var projectDto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            ManagerId = project.ManagerId
        };
        
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
    }

    /// <summary>
    /// Deletes a specific project from the database by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is an ActionResult, returning NoContent if the deletion was successful,
    /// or NotFound if the project does not exist.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}