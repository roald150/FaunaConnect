using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HuntingGroundsController(FaunaDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HuntingGround>>> GetHuntingGrounds([FromQuery] int? userId)
    {
        var query = context.HuntingGrounds.AsQueryable();
        if (userId.HasValue)
        {
            query = query.Where(h => h.UserId == userId.Value);
        }
        return await query.ToListAsync();
    }

    [Authorize(Roles = "Hunter,Admin")]
    [HttpPost]
    public async Task<ActionResult<HuntingGround>> Create(HuntingGround huntingGround)
    {
        context.HuntingGrounds.Add(huntingGround);
        await context.SaveChangesAsync();
        return Ok(huntingGround);
    }

    [Authorize(Roles = "Hunter,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var hg = await context.HuntingGrounds.FindAsync(id);
        if (hg == null) return NotFound();
        context.HuntingGrounds.Remove(hg);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
