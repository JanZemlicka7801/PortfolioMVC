using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManagerController : ControllerBase
{
    private readonly AppDbContext _context;

    public ManagerController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a list of all managers from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing an ActionResult.
    /// The result includes an IEnumerable of ManagerDto objects representing the managers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManagerDto>>> GetManagers()
    {
        var managers = await _context.Managers
            .Select(m => new ManagerDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Department = m.Department,
                Picture = m.Picture
            })
            .ToListAsync();

        return Ok(managers);
    }

    /// <summary>
    /// Retrieves a specific manager by their unique identifier from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the manager to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation, containing an ActionResult.
    /// The result includes a ManagerDto object representing the manager if found, or a NotFound result if no manager exists with the provided identifier.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ManagerDto>> GetManager(int id)
    {
        var manager = await _context.Managers
            .Where(m => m.Id == id)
            .Select(m => new ManagerDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Department = m.Department,
                Picture = m.Picture
            })
            .FirstOrDefaultAsync();

        if (manager == null)
        {
            return NotFound();
        }

        return Ok(manager);
    }

    /// <summary>
    /// Creates a new manager and saves it into the database.
    /// </summary>
    /// <param name="managerDto">The data transfer object containing the details of the manager to create.</param>
    /// <returns>A task that represents the asynchronous operation, containing an ActionResult.
    /// The result includes a ManagerDto object representing the created manager.</returns>
    [HttpPost]
    public async Task<ActionResult<ManagerDto>> CreateManager([FromBody] ManagerDto managerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var manager = new Manager
        {
            Name = managerDto.Name,
            Email = managerDto.Email,
            Department = managerDto.Department,
            Picture = managerDto.Picture
        };

        _context.Managers.Add(manager);
        await _context.SaveChangesAsync();

        var newManagerDto = new ManagerDto
        {
            Id = manager.Id,
            Name = manager.Name,
            Email = manager.Email,
            Department = manager.Department,
            Picture = manager.Picture
        };

        return CreatedAtAction(nameof(GetManager), new { id = manager.Id }, newManagerDto);
    }

    /// <summary>
    /// Updates an existing manager's information in the database.
    /// </summary>
    /// <param name="id">The unique identifier of the manager to update.</param>
    /// <param name="managerDto">The data transfer object containing updated manager details.</param>
    /// <returns>An IActionResult indicating the outcome of the update operation. Returns NoContent if successful,
    /// BadRequest if the id does not match the DTO, or NotFound if the manager does not exist.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManager(int id, [FromBody] ManagerDto managerDto)
    {
        if (id != managerDto.Id)
        {
            return BadRequest();
        }

        var manager = await _context.Managers.FindAsync(id);
        if (manager == null)
        {
            return NotFound();
        }

        manager.Name = managerDto.Name;
        manager.Email = managerDto.Email;
        manager.Department = managerDto.Department;
        manager.Picture = managerDto.Picture;

        _context.Managers.Update(manager);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a manager from the database by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the manager to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The result is an IActionResult indicating the success or failure of the operation,
    /// returning a NotFound result if the manager does not exist, or a NoContent result upon successful deletion.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        var manager = await _context.Managers.FindAsync(id);
        if (manager == null)
        {
            return NotFound();
        }

        _context.Managers.Remove(manager);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}