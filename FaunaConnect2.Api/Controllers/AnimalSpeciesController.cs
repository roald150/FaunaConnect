using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnimalSpeciesController(FaunaDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnimalSpecies>>> Get()
    {
        return await context.AnimalSpecies.ToListAsync();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AnimalSpecies>> Create(AnimalSpecies species)
    {
        context.AnimalSpecies.Add(species);
        await context.SaveChangesAsync();
        return Ok(species);
    }
}
