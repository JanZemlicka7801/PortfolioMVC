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

        /// <summary>
        /// Retrieves all team members from the data source.
        /// </summary>
        /// <returns>Returns a collection of team members as DTOs.</returns>
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

        /// <summary>
        /// Retrieves a team member by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the team member to retrieve.</param>
        /// <returns>Returns the team member as a DTO if found; otherwise, returns null.</returns>
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

        /// <summary>
        /// Creates a new team member and adds it to the data source.
        /// </summary>
        /// <param name="teamMemberDto">An object containing the details of the team member to be created.</param>
        /// <returns>Returns the created team member with the generated unique ID included in the returned DTO.</returns>
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

        /// <summary>
        /// Updates an existing team member's information based on the provided ID and updated data.
        /// </summary>
        /// <param name="id">The ID of the team member to be updated. This must match the ID in the provided data.</param>
        /// <param name="teamMemberDto">An object containing the updated information for the team member.</param>
        /// <returns>Returns true if the update was successful; otherwise, returns false if the IDs do not match or the team member is not found.</returns>
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

        /// <summary>
        /// Deletes a team member with the specified ID from the database.
        /// </summary>
        /// <param name="id">The ID of the team member to be deleted.</param>
        /// <returns>Returns true if the deletion was successful; otherwise, returns false if the team member is not found.</returns>
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
