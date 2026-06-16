using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalSpeciesController : ControllerBase
{
    private readonly FaunaDbContext _context;

    public AnimalSpeciesController(FaunaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnimalSpecies>>> Get()
    {
        return await _context.AnimalSpecies.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<AnimalSpecies>> Create(AnimalSpecies species)
    {
        _context.AnimalSpecies.Add(species);
        await _context.SaveChangesAsync();
        return Ok(species);
    }
}