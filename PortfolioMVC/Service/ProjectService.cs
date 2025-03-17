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