using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;
using FaunaConnect2.Api.Services;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HuntingGroundsController(IHuntingGroundService huntingGroundService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HuntingGroundResource>>> GetHuntingGrounds([FromQuery] int? userId)
    {
        return Ok(await huntingGroundService.GetAllAsync(userId));
    }

    [Authorize(Roles = "Hunter,Admin")]
    [HttpPost]
    public async Task<ActionResult<HuntingGroundResource>> Create(HuntingGround huntingGround)
    {
        return Ok(await huntingGroundService.CreateAsync(huntingGround));
    }

    [Authorize(Roles = "Hunter,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await huntingGroundService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
