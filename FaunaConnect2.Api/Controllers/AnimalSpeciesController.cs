using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;
using FaunaConnect2.Api.Services;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnimalSpeciesController(IAnimalSpeciesService animalSpeciesService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnimalSpeciesResource>>> Get()
    {
        return await animalSpeciesService.GetAllAsync();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AnimalSpeciesResource>> Create(AnimalSpecies species)
    {
        return Ok(await animalSpeciesService.CreateAsync(species));
    }
}
