using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all projects.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of ProjectDto objects representing all projects.</returns>
        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _context.Projects.ToListAsync();
            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ManagerId = p.ManagerId
            });
        }

        /// <summary>
        /// Retrieves a project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ProjectDto object if the project is found; otherwise, null.</returns>
        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return null;

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ManagerId = project.ManagerId
            };
        }

        /// <summary>
        /// Creates a new project using the details provided in the ProjectDto object.
        /// </summary>
        /// <param name="projectDto">The project data transfer object containing the details of the project to be created.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ProjectDto object with the assigned unique identifier after creation.</returns>
        public async Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto)
        {
            var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                StartDate = projectDto.StartDate,
                EndDate = projectDto.EndDate,
                ManagerId = projectDto.ManagerId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Return the DTO with the generated Id
            projectDto.Id = project.Id;
            return projectDto;
        }

        /// <summary>
        /// Updates an existing project with new details provided in the ProjectDto object.
        /// </summary>
        /// <param name="id">The unique identifier of the project to be updated.</param>
        /// <param name="projectDto">The project data transfer object containing the updated project details.</param>
        /// <returns>A task that represents the asynchronous update operation. The task result contains a boolean value indicating whether the update was successful (true) or the project was not found or the IDs did not match (false).</returns>
        public async Task<bool> UpdateProjectAsync(int id, ProjectDto projectDto)
        {
            if (id != projectDto.Id)
                return false;

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            project.Name = projectDto.Name;
            project.Description = projectDto.Description;
            project.StartDate = projectDto.StartDate;
            project.EndDate = projectDto.EndDate;
            project.ManagerId = projectDto.ManagerId;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Deletes a project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project to be deleted.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result contains a boolean value indicating whether the deletion was successful (true) or the project was not found (false).</returns>
        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}