using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Services;

namespace PortfolioMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamMemberController : ControllerBase
    {
        private readonly ITeamMemberService _teamMemberService;

        public TeamMemberController(ITeamMemberService teamMemberService)
        {
            _teamMemberService = teamMemberService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamMemberDto>>> GetAllTeamMembers()
        {
            var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();
            return Ok(teamMembers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamMemberDto>> GetTeamMember(int id)
        {
            var teamMember = await _teamMemberService.GetTeamMemberByIdAsync(id);
            if (teamMember == null)
                return NotFound();
            return Ok(teamMember);
        }

        [HttpPost]
        public async Task<ActionResult<TeamMemberDto>> CreateTeamMember([FromBody] TeamMemberDto teamMemberDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdTeamMember = await _teamMemberService.CreateTeamMemberAsync(teamMemberDto);
            return CreatedAtAction(nameof(GetTeamMember), new { id = createdTeamMember.Id }, createdTeamMember);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeamMember(int id, [FromBody] TeamMemberDto teamMemberDto)
        {
            if (id != teamMemberDto.Id)
                return BadRequest("ID mismatch");

            var updated = await _teamMemberService.UpdateTeamMemberAsync(id, teamMemberDto);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamMember(int id)
        {
            var deleted = await _teamMemberService.DeleteTeamMemberAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}