using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Controllers;

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
    /// Retrieves a list of all team members.
    /// </summary>
    /// <returns>An ActionResult containing an enumerable collection of team member details.</returns>
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
    /// Retrieves a specific team member by their ID.
    /// </summary>
    /// <param name="id">The ID of the team member to retrieve.</param>
    /// <returns>An ActionResult containing the team member details if found; otherwise, a NotFound result.</returns>
    [HttpGet]
    public async Task<ActionResult<TeamMemberDto>> GetTeamMember(int id)
    {
        var teamMember = await _context.TeamMembers.Where(tm => tm.Id == id).Select(tm => new TeamMemberDto
        {
            Id = tm.Id,
            Name = tm.Name,
            Role = tm.Role,
            Email = tm.Email,
            ProjectId = tm.ProjectId
        }).FirstOrDefaultAsync();

        if (teamMember == null)
        {
            return NotFound();
        }

        return Ok(teamMember);
    }

    /// <summary>
    /// Creates a new team member and saves it to the database.
    /// </summary>
    /// <param name="teamMemberDto">The data transfer object containing the details of the team member to create.</param>
    /// <returns>An ActionResult containing the created team member details, along with a CreatedAtAction response if successful.</returns>
    [HttpPost]
    // [FromBody] mean that the object should be provided int the requested body such as JSON
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

        var value = new TeamMemberDto
        {
            Id = teamMember.Id,
            Name = teamMember.Name,
            Role = teamMember.Role,
            Email = teamMember.Email,
            ProjectId = teamMember.ProjectId
        };
        
        return CreatedAtAction(nameof(GetTeamMember), new { id = teamMember.Id }, value);
    }

    /// <summary>
    /// Updates the details of an existing team member.
    /// </summary>
    /// <param name="id">The identifier of the team member to update.</param>
    /// <param name="teamMemberDto">The updated information for the team member.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
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
        teamMemberDto.ProjectId = teamMember.ProjectId;
        
        _context.TeamMembers.Update(teamMember);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    /// <summary>
    /// Deletes a team member based on their ID.
    /// </summary>
    /// <param name="id">The ID of the team member to delete.</param>
    /// <returns>An IActionResult indicating the result of the operation. Returns NoContent if successful, or NotFound if the team member does not exist.</returns>
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