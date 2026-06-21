using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;
using FaunaConnect2.Api.Services;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RegistrationsController(IRegistrationService registrationService) : ControllerBase
{
    [Authorize(Roles = "Hunter,Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistrationResource>>> GetAll()
    {
        return Ok(await registrationService.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<RegistrationResource>> Create([FromBody] Registration newReg)
    {
        var created = await registrationService.CreateAsync(newReg);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }
}
