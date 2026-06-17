namespace FaunaConnect2.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RegistrationsController(FaunaDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves all wildlife registrations.
    /// </summary>
    /// <remarks>
    /// Requires Hunter or Admin role.
    /// </remarks>
    /// <returns>A list of registrations including user details.</returns>
    [Authorize(Roles = "Hunter,Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Registration>>> GetAll()
    {
        var data = await context.Registrations
            .Include(r => r.User) // Add hunter details directly (SQL JOIN)
            .ToListAsync();
            
        return Ok(data);
    }

    /// <summary>
    /// Creates a new wildlife registration.
    /// </summary>
    /// <param name="newReg">The registration data.</param>
    /// <returns>The created registration.</returns>
    [HttpPost]
    public async Task<ActionResult<Registration>> Create([FromBody] Registration newReg)
    {
        // Set the time to now
        newReg.DateTime = DateTime.Now;

        // Add to the EF Core collection
        context.Registrations.Add(newReg);
        
        // Persist changes to SQL Server
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = newReg.Id }, newReg);
    }
}
