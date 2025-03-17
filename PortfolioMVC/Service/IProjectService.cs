using PortfolioMVC.Models.DTOs;

namespace PortfolioMVC.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto?> GetProjectByIdAsync(int id);
        Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto);
        Task<bool> UpdateProjectAsync(int id, ProjectDto projectDto);
        Task<bool> DeleteProjectAsync(int id);
    }
}