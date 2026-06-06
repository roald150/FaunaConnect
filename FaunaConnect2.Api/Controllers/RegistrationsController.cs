namespace FaunaConnect2.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly FaunaDbContext _context;

    // EF Core wordt hier automatisch geïnjecteerd via de constructor
    public RegistrationsController(FaunaDbContext context)
    {
        _context = context;
    }

    // GET: api/registrations (Haal alles op uit de SQL database)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Registration>>> GetAll()
    {
        var data = await _context.Registrations
            .Include(r => r.User) // Voeg de jagergegevens direct toe (SQL JOIN)
            .ToListAsync();
            
        return Ok(data);
    }

    // POST: api/registrations (Sla écht op in de database)
    [HttpPost]
    public async Task<ActionResult<Registration>> Create([FromBody] Registration newReg)
    {
        // We zetten de tijd op nu
        newReg.DateTime = DateTime.Now;

        // Voeg toe aan de EF Core verzameling
        _context.Registrations.Add(newReg);
        
        // Schrijf de wijzigingen definitief weg naar SQL Server
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = newReg.Id }, newReg);
    }
}