using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly AppDbContext _context;

        public TeamMemberService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeamMemberDto>> GetAllTeamMembersAsync()
        {
            var teamMembers = await _context.TeamMembers.ToListAsync();
            return teamMembers.Select(tm => new TeamMemberDto
            {
                Id = tm.Id,
                Name = tm.Name,
                Role = tm.Role,
                Email = tm.Email,
                ProjectId = tm.ProjectId
            });
        }

        public async Task<TeamMemberDto?> GetTeamMemberByIdAsync(int id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
                return null;

            return new TeamMemberDto
            {
                Id = teamMember.Id,
                Name = teamMember.Name,
                Role = teamMember.Role,
                Email = teamMember.Email,
                ProjectId = teamMember.ProjectId
            };
        }

        public async Task<TeamMemberDto> CreateTeamMemberAsync(TeamMemberDto teamMemberDto)
        {
            var teamMember = new TeamMember
            {
                Name = teamMemberDto.Name,
                Role = teamMemberDto.Role,
                Email = teamMemberDto.Email,
                ProjectId = teamMemberDto.ProjectId
            };

            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();

            // Return the DTO with the generated ID
            teamMemberDto.Id = teamMember.Id;
            return teamMemberDto;
        }

        public async Task<bool> UpdateTeamMemberAsync(int id, TeamMemberDto teamMemberDto)
        {
            if (id != teamMemberDto.Id)
                return false;

            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
                return false;

            teamMember.Name = teamMemberDto.Name;
            teamMember.Role = teamMemberDto.Role;
            teamMember.Email = teamMemberDto.Email;
            teamMember.ProjectId = teamMemberDto.ProjectId;

            _context.TeamMembers.Update(teamMember);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTeamMemberAsync(int id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
                return false;

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
