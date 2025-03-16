using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMemberController : ControllerBase
    {
        private readonly AppDbContext _context;
    
        public TeamMemberController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all team members in the database.
        /// </summary>
        /// <returns>An ActionResult containing a list of TeamMemberDto objects representing all team members. Returns 200 OK with the list of team members.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamMemberDto>>> GetTeamMembers()
        {
            var teamMembers = await _context.TeamMembers.Select(tm => new TeamMemberDto
            {
                Id = tm.Id,
                Name = tm.Name,
                Role = tm.Role,
                Email = tm.Email,
                ProjectId = tm.ProjectId
            }).ToListAsync();
    
            return Ok(teamMembers);
        }

        /// <summary>
        /// Retrieves a specific team member by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the team member to retrieve.</param>
        /// <returns>An ActionResult containing the requested team member's data. Returns 200 OK if the team member is found, or 404 Not Found if the team member does not exist.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamMemberDto>> GetTeamMember(int id)
        {
            var teamMember = await _context.TeamMembers
                .Where(tm => tm.Id == id)
                .Select(tm => new TeamMemberDto
                {
                    Id = tm.Id,
                    Name = tm.Name,
                    Role = tm.Role,
                    Email = tm.Email,
                    ProjectId = tm.ProjectId
                })
                .FirstOrDefaultAsync();
    
            if (teamMember == null)
            {
                return NotFound();
            }
    
            return Ok(teamMember);
        }

        /// <summary>
        /// Creates a new team member and adds it to the database.
        /// </summary>
        /// <param name="teamMemberDto">The data transfer object containing the details of the team member to be created.</param>
        /// <returns>An ActionResult containing the created team member's data. Returns 201 Created with the newly created TeamMemberDto if the operation is successful, or 400 Bad Request if the request data is invalid.</returns>
        [HttpPost]
        public async Task<ActionResult<TeamMemberDto>> CreateTeamMember([FromBody] TeamMemberDto teamMemberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
    
            var teamMember = new TeamMember
            {
                Name = teamMemberDto.Name,
                Role = teamMemberDto.Role,
                Email = teamMemberDto.Email,
                ProjectId = teamMemberDto.ProjectId
            };
            
            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();
    
            var createdDto = new TeamMemberDto
            {
                Id = teamMember.Id,
                Name = teamMember.Name,
                Role = teamMember.Role,
                Email = teamMember.Email,
                ProjectId = teamMember.ProjectId
            };
            
            return CreatedAtAction(nameof(GetTeamMember), new { id = teamMember.Id }, createdDto);
        }

        /// <summary>
        /// Updates the details of an existing team member in the database with the specified ID.
        /// </summary>
        /// <param name="id">The unique ID of the team member to update.</param>
        /// <param name="teamMemberDto">The data transfer object containing the updated team member information.</param>
        /// <returns>An IActionResult indicating the outcome of the update operation. Returns 204 No Content if successful, or 400 Bad Request if the ID does not match, or 404 Not Found if the team member does not exist.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeamMember(int id, [FromBody] TeamMemberDto teamMemberDto)
        {
            if (id != teamMemberDto.Id)
            {
                return BadRequest();
            }
            
            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }
            
            teamMember.Name = teamMemberDto.Name;
            teamMember.Role = teamMemberDto.Role;
            teamMember.Email = teamMemberDto.Email;
            teamMember.ProjectId = teamMemberDto.ProjectId;
            
            _context.TeamMembers.Update(teamMember);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        /// <summary>
        /// Deletes a team member from the database identified by the specified ID.
        /// </summary>
        /// <param name="id">The unique ID of the team member to delete.</param>
        /// <returns>A status code indicating the outcome of the operation. Returns 204 No Content if successful, or 404 Not Found if the team member does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamMember(int id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }
            
            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
