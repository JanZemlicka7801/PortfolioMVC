using PortfolioMVC.Models.DTOs;

namespace PortfolioMVC.Services
{
    public interface ITeamMemberService
    {
        Task<IEnumerable<TeamMemberDto>> GetAllTeamMembersAsync();
        Task<TeamMemberDto?> GetTeamMemberByIdAsync(int id);
        Task<TeamMemberDto> CreateTeamMemberAsync(TeamMemberDto teamMemberDto);
        Task<bool> UpdateTeamMemberAsync(int id, TeamMemberDto teamMemberDto);
        Task<bool> DeleteTeamMemberAsync(int id);
    }
}