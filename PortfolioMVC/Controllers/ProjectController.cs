using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Controllers
{
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
        /// Retrieves a list of all projects from the database.
        /// </summary>
        /// <returns>A list of ProjectDto objects representing the projects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
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
        /// Retrieves a project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <returns>A ProjectDto object representing the project if found; otherwise, a NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
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
        /// Creates a new project and stores it in the database.
        /// </summary>
        /// <param name="createProjectDto">An object containing the data required to create the project.</param>
        /// <returns>The created ProjectDto object including its generated ID.</returns>
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] ProjectDto createProjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var project = new Project
            {
                // Id is auto-generated
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                StartDate = createProjectDto.StartDate,
                EndDate = createProjectDto.EndDate,
                ManagerId = createProjectDto.ManagerId
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
        /// Updates the details of an existing project in the database based on the provided project information.
        /// </summary>
        /// <param name="id">The unique identifier of the project to be updated.</param>
        /// <param name="updateProjectDto">The updated project information encapsulated in a ProjectDto object.</param>
        /// <returns>An IActionResult indicating the result of the update operation, such as NoContent if successful or error responses for invalid conditions.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, [FromBody] ProjectDto updateProjectDto)
        {
            if (id != updateProjectDto.Id)
            {
                return BadRequest();
            }
    
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            project.Name = updateProjectDto.Name;
            project.Description = updateProjectDto.Description;
            project.StartDate = updateProjectDto.StartDate;
            project.EndDate = updateProjectDto.EndDate;
            project.ManagerId = updateProjectDto.ManagerId;
            
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
    
            return NoContent();
        }

        /// <summary>
        /// Deletes a project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project to be deleted.</param>
        /// <returns>An IActionResult indicating the result of the delete operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
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
}
