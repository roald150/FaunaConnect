using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HuntingGroundsController : ControllerBase
{
    private readonly FaunaDbContext _context;

    public HuntingGroundsController(FaunaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HuntingGround>>> GetHuntingGrounds([FromQuery] int? userId)
    {
        var query = _context.HuntingGrounds.AsQueryable();
        if (userId.HasValue)
        {
            query = query.Where(h => h.UserId == userId.Value);
        }
        return await query.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<HuntingGround>> Create(HuntingGround huntingGround)
    {
        _context.HuntingGrounds.Add(huntingGround);
        await _context.SaveChangesAsync();
        return Ok(huntingGround);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var hg = await _context.HuntingGrounds.FindAsync(id);
        if (hg == null) return NotFound();
        _context.HuntingGrounds.Remove(hg);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}